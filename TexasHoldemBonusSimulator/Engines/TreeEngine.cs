using BerldPokerEngine.Poker;
using BerldPokerEngine;
using static TexasHoldemBonusSimulator.Engines.EngineHelper;

namespace TexasHoldemBonusSimulator.Engines
{
    internal class TreeEngine
    {
        internal const long PreflopTreeIterationAmount = PreflopIterationAmount * FlopTreeIterationAmount; // 55_627_620_048_000L
        private const long FlopTreeIterationAmount = FlopIterationAmount * (long)TurnTreeIterationAmount; // 41_951_448_000L
        private const int TurnTreeIterationAmount = TurnIterationAmount * ResultIterationAmount; // 2_140_380

        private const int PreflopIterationAmount = 1_326;
        private const int FlopIterationAmount = 19_600;
        private const int TurnIterationAmount = 47;
        private const int ResultIterationAmount = 45_540;

        internal static long EvaluatePreflopTree(int ante)
        {
            Card[] aliveCards = EngineData.GetAllCards().ToArray();

            List<int> sortMarkers = new() { 0, 2 };
            Dictionary<string, DistinctHolding> holdingMap = new();

            for (int p1 = 0; p1 < aliveCards.Length; p1++)
            {
                for (int p2 = p1 + 1; p2 < aliveCards.Length; p2++)
                {
                    Card[] cards = new Card[]
                    {
                        aliveCards[p1],
                        aliveCards[p2]
                    };

                    DistinctHolding holding = new(cards, sortMarkers);

                    if (holdingMap.ContainsKey(holding.Key))
                    {
                        holdingMap[holding.Key].Frequency++;
                    }
                    else
                    {
                        holding.Frequency = 1;
                        holdingMap.Add(holding.Key, holding);
                    }
                }
            }

            long winnings = 0;

            foreach (DistinctHolding holding in holdingMap.Values)
            {
                List<Card> playerCards = new()
                {
                    holding.Cards[0],
                    holding.Cards[1]
                };

                Console.WriteLine($"On {playerCards[0]}{playerCards[1]}");
                long preflopWinnings = EvaluateFlopTree(playerCards, ante);
                long foldWinnings = FlopTreeIterationAmount * -ante;

                if (foldWinnings >= preflopWinnings)
                {
                    winnings += holding.Frequency * foldWinnings;
                    Console.WriteLine($"Folded {playerCards[0]}{playerCards[1]}");
                }
                else
                {
                    winnings += holding.Frequency * preflopWinnings;
                }
            }

            return winnings;
        }

        internal static long EvaluateFlopTree(List<Card> playerCards, int ante)
        {
            Card[] aliveCards = EngineData.GetAllCards().Except(playerCards).ToArray();

            List<int> sortMarkers = new() { 0, 2, 5 };
            Dictionary<string, DistinctHolding> holdingMap = new();

            for (int f1 = 0; f1 < aliveCards.Length; f1++)
            {
                for (int f2 = f1 + 1; f2 < aliveCards.Length; f2++)
                {
                    for (int f3 = f2 + 1; f3 < aliveCards.Length; f3++)
                    {
                        Card[] cards = new Card[]
                        {
                            playerCards[0],
                            playerCards[1],
                            aliveCards[f1],
                            aliveCards[f2],
                            aliveCards[f3]
                        };

                        DistinctHolding holding = new(cards, sortMarkers);

                        if (holdingMap.ContainsKey(holding.Key))
                        {
                            holdingMap[holding.Key].Frequency++;
                        }
                        else
                        {
                            holding.Frequency = 1;
                            holdingMap.Add(holding.Key, holding);
                        }
                    }
                }
            }

            DistinctHolding[] holdings = holdingMap.Values.ToArray();

            Console.WriteLine(holdings.Length);

            CancellationTokenSource tokenSource = new();

            Task reportingTask = Task.Run(async () =>
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    await Task.Delay(10_000);
                    if (tokenSource.IsCancellationRequested) return;
                    ReportProgress(holdings);
                }
            });

            var result = Parallel.For(0, holdings.Length, i =>
            {
                DistinctHolding holding = holdings[i];

                List<Card> flopBoardCards = new()
                {
                    holding.Cards[2],
                    holding.Cards[3],
                    holding.Cards[4]
                };

                bool isFlopBet = DecisionEngine.EvaluateFlop(playerCards, flopBoardCards);
                int flopRestBet = ante * 2;

                if (isFlopBet)
                {
                    flopRestBet += ante;
                }

                holding.Winnings += holding.Frequency * EvaluateTurnTree(playerCards, flopBoardCards, ante, flopRestBet);
                holding.WinningsCalculated = true;
            });

            tokenSource.Cancel();

            return holdings.Sum(c => c.Winnings);
        }

        internal static int EvaluateTurnTree(List<Card> playerCards, List<Card> boardCards, int ante, int restBet)
        {
            IEnumerable<Card> deadCards = Enumerable.Concat(playerCards, boardCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            int winnings = 0;

            for (int b4 = 0; b4 < aliveCards.Length; b4++)
            {
                Card turnCard = aliveCards[b4];

                List<Card> turnBoardCards = new(boardCards)
                {
                    turnCard
                };

                bool isTurnBet = DecisionEngine.EvaluateTurn(playerCards, turnBoardCards);
                int turnRestBet = restBet;

                if (isTurnBet)
                {
                    turnRestBet += ante;
                }

                winnings += EvaluateResult(playerCards, turnBoardCards, ante, turnRestBet);
            }

            return winnings;
        }

        internal static int EvaluateResult(List<Card> playerCards, List<Card> boardCards, int ante, int restBet)
        {
            IEnumerable<Card> deadCards = Enumerable.Concat(playerCards, boardCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            Card[] playerAllCards = new Card[HandValueCardAmount];
            HandValue playerValue = new();

            Card[] dealerAllCards = new Card[HandValueCardAmount];
            HandValue dealerValue = new();

            playerAllCards[0] = playerCards[0];
            playerAllCards[1] = playerCards[1];

            for (int i = 0; i < boardCards.Count; i++)
            {
                playerAllCards[PlayerCardAmount + i] = boardCards[i];
                dealerAllCards[PlayerCardAmount + i] = boardCards[i];
            }

            int winnings = 0;

            for (int b5 = 0; b5 < aliveCards.Length; b5++)
            {
                for (int d1 = 0; d1 < aliveCards.Length; d1++)
                {
                    if (d1 == b5) continue;

                    for (int d2 = d1 + 1; d2 < aliveCards.Length; d2++)
                    {
                        if (d2 == b5) continue;

                        dealerAllCards[0] = aliveCards[d1];
                        dealerAllCards[1] = aliveCards[d2];

                        playerAllCards[6] = aliveCards[b5];
                        dealerAllCards[6] = aliveCards[b5];

                        Engine.SetHandValue(playerAllCards, playerValue);
                        Engine.SetHandValue(dealerAllCards, dealerValue);

                        int comparison = playerValue.CompareTo(dealerValue);

                        if (comparison > 0)
                        {
                            if (playerValue.Hand >= Hand.Straight)
                            {
                                winnings += ante;
                            }

                            winnings += restBet;
                        }
                        else
                        {
                            winnings -= ante;
                            winnings -= restBet;
                        }
                    }
                }
            }

            return winnings;
        }

        private static void ReportProgress(DistinctHolding[] holdings)
        {
            int holdingsDone = holdings.Count(c => c.WinningsCalculated);
            long winnings = holdings.Sum(c => c.Winnings);
            double progressPercent = holdingsDone / (double)holdings.Length * 100;
            double averageWinnings = winnings / (double)FlopTreeIterationAmount;

            Console.WriteLine($"Progress: {progressPercent:0.00}%");
            Console.WriteLine($"Winnings: {winnings}");
            Console.WriteLine($"Average winnings: {averageWinnings:G4}");
            Console.WriteLine();
        }
    }
}
