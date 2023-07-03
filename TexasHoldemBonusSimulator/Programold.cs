using BerldPokerEngine;
using BerldPokerEngine.Poker;
using System.Diagnostics;

namespace TexasHoldemBonusSimulator
{
    internal class Program
    {
        private const int PlayerCardAmount = 2;
        private const int HandValueCardAmount = 7;

        private const long PreflopTreeIterationAmount = PreflopIterationAmount * FlopTreeIterationAmount; // 55_627_620_048_000L
        private const long FlopTreeIterationAmount = FlopIterationAmount * (long)TurnTreeIterationAmount; // 41_951_448_000L
        private const int TurnTreeIterationAmount = TurnIterationAmount * ResultIterationAmount; // 2_140_380

        private const int PreflopIterationAmount = 1_326;
        private const int FlopIterationAmount = 19_600;
        private const int TurnIterationAmount = 47;
        private const int ResultIterationAmount = 45_540;

        private static void Main()
        {
            const int Ante = 1;

            long winnings = EvaluatePreflopTree(Ante);

            double averageWinnings = winnings / (double)PreflopTreeIterationAmount;
            string signText = averageWinnings > 0 ? "win" : "loss";

            Console.WriteLine($"Average {signText} of {Math.Abs(averageWinnings):0.00} times the ante");
        }

        private static long EvaluatePreflopTree(int ante)
        {
            Card[] aliveCards = EngineData.GetAllCards().ToArray();
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

                    DistinctHolding holding = new(cards, new() { 0, 2 });

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

            foreach (DistinctHolding holding in holdingMap.Values.Skip(40))
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

        private static long EvaluateFlopTree(List<Card> playerCards, int ante)
        {
            Debug.Assert(playerCards.Count == PlayerCardAmount);

            Card[] aliveCards = EngineData.GetAllCards().Except(playerCards).ToArray();
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

                        DistinctHolding holding = new(cards, new() { 0, 2, 5 });

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

            long winnings = 0;

            int done = 0;
            int todo = holdingMap.Values.Count;

            foreach (DistinctHolding holding in holdingMap.Values)
            {
                List<Card> flopBoardCards = new()
                {
                    holding.Cards[2],
                    holding.Cards[3],
                    holding.Cards[4]
                };

                bool isFlopBet = EvaluateFlop(playerCards, flopBoardCards);
                int flopRestBet = ante * 2;

                if (isFlopBet)
                {
                    flopRestBet += ante;
                }

                winnings += holding.Frequency * EvaluateTurnTree(playerCards, flopBoardCards, ante, flopRestBet);

                if (done > 0 && done % 10 == 0)
                {
                    Console.WriteLine($"done {done} of {todo}");
                }

                done++;
            }

            return winnings;
        }

        private static int EvaluateTurnTree(List<Card> playerCards, List<Card> boardCards, int ante, int restBet)
        {
            const int BoardCardAmount = 3;

            Debug.Assert(playerCards.Count == PlayerCardAmount);
            Debug.Assert(boardCards.Count == BoardCardAmount);

            IEnumerable<Card> deadCards = Enumerable.Concat(playerCards, boardCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            Dictionary<string, DistinctHolding> holdingMap = new();

            for (int b4 = 0; b4 < aliveCards.Length; b4++)
            {
                Card[] cards = new Card[]
                {
                    playerCards[0],
                    playerCards[1],
                    boardCards[0],
                    boardCards[1],
                    boardCards[2],
                    aliveCards[b4]
                };

                DistinctHolding holding = new(cards, new() { 0, 2, 5, 6 });

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

            int winnings = 0;

            //if (holdingMap.Values.Count != TurnIterationAmount)
            //{
            //    Console.WriteLine($"holdingMap.Values.Count: {holdingMap.Values.Count}");
            //}

            foreach (DistinctHolding holding in holdingMap.Values)
            {
                Card turnCard = holding.Cards[5];

                List<Card> turnBoardCards = new(boardCards)
                {
                    turnCard
                };

                bool isTurnBet = EvaluateTurn(playerCards, turnBoardCards);
                int turnRestBet = restBet;

                if (isTurnBet)
                {
                    turnRestBet += ante;
                }

                winnings += holding.Frequency * EvaluateResult(playerCards, turnBoardCards, ante, turnRestBet);
            }

            return winnings;
        }

        private static bool EvaluateFlop(List<Card> playerCards, List<Card> boardCards)
        {
            const int IterationAmount = 1_070_190;
            const int WinScoreThreshold = IterationAmount;
            const int BoardCardAmount = 3;

            Debug.Assert(playerCards.Count == PlayerCardAmount);
            Debug.Assert(boardCards.Count == BoardCardAmount);

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

            int iterationsLeft = IterationAmount;
            int winScore = 0;

            for (int b4 = 0; b4 < aliveCards.Length; b4++)
            {
                for (int b5 = b4 + 1; b5 < aliveCards.Length; b5++)
                {
                    for (int d1 = 0; d1 < aliveCards.Length; d1++)
                    {
                        if (d1 == b4 || d1 == b5) continue;

                        for (int d2 = d1 + 1; d2 < aliveCards.Length; d2++)
                        {
                            if (d2 == b4 || d2 == b5) continue;

                            dealerAllCards[0] = aliveCards[d1];
                            dealerAllCards[1] = aliveCards[d2];

                            playerAllCards[5] = aliveCards[b4];
                            playerAllCards[6] = aliveCards[b5];

                            dealerAllCards[5] = aliveCards[b4];
                            dealerAllCards[6] = aliveCards[b5];

                            Engine.SetHandValue(playerAllCards, playerValue);
                            Engine.SetHandValue(dealerAllCards, dealerValue);

                            int comparison = playerValue.CompareTo(dealerValue);

                            if (comparison > 0)
                            {
                                winScore += 2;
                            }
                            else if (comparison == 0)
                            {
                                winScore += 1;
                            }

                            if (winScore > WinScoreThreshold)
                            {
                                return true;
                            }
                            else if (winScore + iterationsLeft * 2 <= WinScoreThreshold)
                            {
                                return false;
                            }

                            iterationsLeft--;
                        }
                    }
                }
            }

            return false;
        }

        private static bool EvaluateTurn(List<Card> playerCards, List<Card> boardCards)
        {
            const int IterationAmount = 45_540;
            const int WinScoreThreshold = IterationAmount;
            const int BoardCardAmount = 4;

            Debug.Assert(playerCards.Count == PlayerCardAmount);
            Debug.Assert(boardCards.Count == BoardCardAmount);

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

            int iterationsLeft = IterationAmount;
            int winScore = 0;

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
                            winScore += 2;
                        }
                        else if (comparison == 0)
                        {
                            winScore += 1;
                        }

                        if (winScore > WinScoreThreshold)
                        {
                            return true;
                        }
                        else if (winScore + iterationsLeft * 2 <= WinScoreThreshold)
                        {
                            return false;
                        }

                        iterationsLeft--;
                    }
                }
            }

            return false;
        }

        private static int EvaluateResult(List<Card> playerCards, List<Card> boardCards, int ante, int restBet)
        {
            const int BoardCardAmount = 4;

            Debug.Assert(playerCards.Count == PlayerCardAmount);
            Debug.Assert(boardCards.Count == BoardCardAmount);

            IEnumerable<Card> deadCards = Enumerable.Concat(playerCards, boardCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            Dictionary<string, DistinctHolding> holdingMap = new();

            for (int b5 = 0; b5 < aliveCards.Length; b5++)
            {
                for (int d1 = 0; d1 < aliveCards.Length; d1++)
                {
                    if (d1 == b5) continue;

                    for (int d2 = d1 + 1; d2 < aliveCards.Length; d2++)
                    {
                        if (d2 == b5) continue;

                        Card[] cards = new Card[]
                        {
                            playerCards[0],
                            playerCards[1],
                            boardCards[0],
                            boardCards[1],
                            boardCards[2],
                            boardCards[3],
                            aliveCards[b5],
                            aliveCards[d1],
                            aliveCards[d2]
                        };

                        DistinctHolding holding = new(cards, new() { 0, 2, 5, 6, 7, 9 });

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

            //if (holdingMap.Values.Count != ResultIterationAmount)
            //{
            //    Console.WriteLine($"holdingMap.Values.Count: {holdingMap.Values.Count}");
            //}

            foreach (DistinctHolding holding in holdingMap.Values)
            {
                dealerAllCards[0] = holding.Cards[7];
                dealerAllCards[1] = holding.Cards[8];

                playerAllCards[6] = holding.Cards[6];
                dealerAllCards[6] = holding.Cards[6];

                Engine.SetHandValue(playerAllCards, playerValue);
                Engine.SetHandValue(dealerAllCards, dealerValue);

                int comparison = playerValue.CompareTo(dealerValue);

                if (comparison > 0)
                {
                    if (playerValue.Hand >= Hand.Straight)
                    {
                        winnings += holding.Frequency * ante;
                    }

                    winnings += holding.Frequency * restBet;
                }
                else
                {
                    winnings -= holding.Frequency * ante;
                    winnings -= holding.Frequency * restBet;
                }
            }

            return winnings;
        }
    }
}