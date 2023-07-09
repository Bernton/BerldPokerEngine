using BerldPokerEngine;
using BerldPokerEngine.Poker;
using System.Linq;

namespace UltimateTexasHoldemSimulator
{
    internal class Program
    {
        private const int Ante = 1;
        private const int Blind = Ante;

        private const long FlopTreeIterations = FlopIterations * (long)RiverTreeIterations; // 20_975_724_000
        private const int RiverTreeIterations = RiverIterations * ResultIterations; // 1_070_190

        private const int FlopIterations = 19600; // 50 choose 3
        private const int RiverIterations = 1081; // 47 choose 2
        private const int ResultIterations = 990; // 45 choose 2

        private static void Main()
        {
            // Ante 1, Blind 1, Play 0

            // See first 2 cards

            // Check, raise 4x or raise 3x

            List<Card> playerCards = new() { Card.Card2c, Card.Card3d };

            EvaluatePreflop(playerCards);

            //(double winnings3, double winnings4) = OnRaise3or4(playerCards);

            //Console.WriteLine($"Winnings 3: {winnings3} times the ante.");
            //Console.WriteLine($"Winnings 4: {winnings4} times the ante.");
        }


        private static (double winnings, long playBetWins) EvaluateResult(List<Card> playerCards, Card[] boardCards)
        {
            List<Card> aliveCards = EngineData.GetAllCards().Except(playerCards).Except(boardCards).ToList();

            HandValue playerValue = new();
            Card[] playerCardsToEvaluate = new Card[7];

            playerCardsToEvaluate[5] = playerCards[0];
            playerCardsToEvaluate[6] = playerCards[1];

            HandValue dealerValue = new();
            Card[] dealerCardsToEvaluate = new Card[7];

            for (int i = 0; i < boardCards.Length; i++)
            {
                playerCardsToEvaluate[i] = boardCards[i];
                dealerCardsToEvaluate[i] = boardCards[i];
            }

            double winnings = 0;
            long playBetWins = 0;

            for (int d1 = 0; d1 < aliveCards.Count; d1++)
            {
                dealerCardsToEvaluate[5] = aliveCards[d1];

                for (int d2 = d1 + 1; d2 < aliveCards.Count; d2++)
                {
                    dealerCardsToEvaluate[6] = aliveCards[d2];

                    Engine.SetHandValue(playerCardsToEvaluate, playerValue);
                    Engine.SetHandValue(dealerCardsToEvaluate, dealerValue);

                    int comparison = playerValue.CompareTo(dealerValue);

                    bool playerWins = comparison > 0;
                    bool dealerWins = comparison < 0;
                    bool dealerQualified = dealerValue.Hand >= Hand.Pair;

                    if (playerWins)
                    {
                        if (dealerQualified)
                        {
                            winnings += Ante;
                        }

                        playBetWins += 1;
                        winnings += Blind * GetBlindMuliplier(playerValue.Hand);
                    }
                    else if (dealerWins)
                    {
                        if (dealerQualified)
                        {
                            winnings -= Ante;
                        }

                        playBetWins -= 1;
                        winnings -= Blind;
                    }
                }
            }

            return (winnings, playBetWins);
        }

        private static (double winnings, long playBetWins, double checkWinnings) EvaluateBoard(List<Card> playerCards, Card[] flopCards)
        {
            const int RiverRaise = 1 * Ante;
            const int FoldWinnings = -(Ante + Blind) * ResultIterations;

            List<Card> aliveCards = EngineData.GetAllCards().Except(playerCards).Except(flopCards).ToList();

            Card[] boardCards = new Card[5];

            for (int i = 0; i < flopCards.Length; i++)
            {
                boardCards[i] = flopCards[i];
            }

            double winnings = 0;
            long playBetWins = 0;
            double checkWinnings = 0;

            for (int b4 = 0; b4 < aliveCards.Count; b4++)
            {
                boardCards[3] = aliveCards[b4];

                for (int b5 = b4 + 1; b5 < aliveCards.Count; b5++)
                {
                    boardCards[4] = aliveCards[b5];

                    (double resultWinnings, long resultPlayBetWins) = EvaluateResult(playerCards, boardCards);

                    winnings += resultWinnings;
                    playBetWins += resultPlayBetWins;

                    double boardRaiseWinnings = resultWinnings + resultPlayBetWins * RiverRaise;
                    checkWinnings += Math.Max(boardRaiseWinnings, FoldWinnings);
                }
            }

            return (winnings, playBetWins, checkWinnings);
        }

        private static void EvaluatePreflop(List<Card> playerCards)
        {
            const int FlopCards = 3;
            const int PreflopRaise3 = 3 * Ante;
            const int PreflopRaise4 = 4 * Ante;
            const int TurnRaise = 2 * Ante;

            Card[] aliveCards = EngineData.GetAllCards().Except(playerCards).ToArray();

            List<int> sortMarkers = new() { 0, 2 };
            Dictionary<string, DistinctFlopHolding> holdingMap = new();

            for (int p1 = 0; p1 < aliveCards.Length; p1++)
            {
                for (int p2 = p1 + 1; p2 < aliveCards.Length; p2++)
                {
                    Card[] holdingCards = new Card[]
                    {
                        aliveCards[p1],
                        aliveCards[p2]
                    };

                    DistinctFlopHolding holding = new(holdingCards, sortMarkers);

                    if (holdingMap.ContainsKey(holding.Key))
                    {
                        DistinctFlopHolding existingHolding = holdingMap[holding.Key];
                        existingHolding.Frequency++;
                    }
                    else
                    {
                        holding.Frequency = 1;
                        holdingMap.Add(holding.Key, holding);
                    }
                }
            }

            DistinctFlopHolding[] holdings = holdingMap.Values.ToArray();

            Parallel.For(0, holdings.Length, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, holdingI =>
            {
                DistinctFlopHolding holding = holdings[holdingI];
                Card[] flopCards = new Card[FlopCards];

                for (int i = 0; i < flopCards.Length; i++)
                {
                    flopCards[i] = holding.Cards[playerCards.Count + i];
                }

                (double boardWinnings, long boardPlayBetWins, double checkFlopWinnings) = EvaluateBoard(playerCards, flopCards);

                double flopRaiseWinnings = boardWinnings + boardPlayBetWins * TurnRaise;
                double checkPreflopWinnings = Math.Max(flopRaiseWinnings, checkFlopWinnings);
                holding.CheckWinnings += holding.Frequency * checkPreflopWinnings;

                holding.Raise3Winnings += holding.Frequency * (boardWinnings + boardPlayBetWins * PreflopRaise3);
                holding.Raise4Winnings += holding.Frequency * (boardWinnings + boardPlayBetWins * PreflopRaise4);

                holding.IsEvaluated = true;

                int flopIterationsDone = holdings.Count(c => c.IsEvaluated);

                if (flopIterationsDone == holdings.Length)
                {
                    Console.WriteLine();
                    Console.WriteLine("--- Results ---");
                }
                else
                {
                    double progressPercent = flopIterationsDone / (double)holdings.Length * 100;
                    Console.WriteLine($"Progress:\t\t{progressPercent,5:0.00}%");
                }

                Console.WriteLine($"Winnings check:\t\t{holdings.Sum(c => c.CheckWinnings) / FlopTreeIterations,10:0.0000000} times the ante");
                Console.WriteLine($"Winnings raise 3:\t{holdings.Sum(c => c.Raise3Winnings) / FlopTreeIterations,10:0.0000000} times the ante");
                Console.WriteLine($"Winnings raise 4:\t{holdings.Sum(c => c.Raise4Winnings) / FlopTreeIterations,10:0.0000000} times the ante");
                Console.WriteLine();
            });
        }

        private static double GetBlindMuliplier(int hand)
        {
            return hand switch
            {
                Hand.RoyalFlush => 500,
                Hand.StraightFlush => 50,
                Hand.FourOfAKind => 10,
                Hand.FullHouse => 3,
                Hand.Flush => 1.5,
                Hand.Straight => 1,
                _ => 0
            };
        }
    }
}