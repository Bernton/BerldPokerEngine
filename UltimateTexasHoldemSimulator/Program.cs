using BerldPokerEngine;
using BerldPokerEngine.Poker;

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

            List<Card> aliveCards = EngineData.GetAllCards().Except(playerCards).ToList();

            double[] f1WinningsRaise4 = new double[aliveCards.Count];
            double[] f1WinningsRaise3 = new double[aliveCards.Count];
            double[] f1WinningsCheck = new double[aliveCards.Count];
            int[] runCounts = new int[aliveCards.Count];

            int f1Iterations = aliveCards.Count - FlopCards + 1;

            Parallel.For(0, f1Iterations/*, new ParallelOptions() { MaxDegreeOfParallelism = 1 }*/, f1 =>
            {
                Card[] flopCards = new Card[FlopCards];
                flopCards[0] = aliveCards[f1];

                for (int f2 = f1 + 1; f2 < aliveCards.Count; f2++)
                {
                    flopCards[1] = aliveCards[f2];

                    for (int f3 = f2 + 1; f3 < aliveCards.Count; f3++)
                    {
                        flopCards[2] = aliveCards[f3];

                        (double boardWinnings, long boardPlayBetWins, double flopCheckWinnings) = EvaluateBoard(playerCards, flopCards);

                        f1WinningsRaise3[f1] += boardWinnings + boardPlayBetWins * PreflopRaise3;
                        f1WinningsRaise4[f1] += boardWinnings + boardPlayBetWins * PreflopRaise4;

                        double flopRaiseWinnings = boardWinnings + boardPlayBetWins * TurnRaise;
                        double flopWinnings = Math.Max(flopRaiseWinnings, flopCheckWinnings);
                        f1WinningsCheck[f1] += flopWinnings;

                        runCounts[f1]++;
                    }
                }

                int flopIterationsDone = runCounts.Sum();

                if (flopIterationsDone == FlopIterations)
                {
                    Console.WriteLine();
                    Console.WriteLine("--- Results ---");
                }
                else
                {
                    double progressPercent = runCounts.Sum() / (double)FlopIterations * 100;
                    Console.WriteLine($"Progress:\t\t{progressPercent,5:0.00}%");
                }

                Console.WriteLine($"Winnings check:\t\t{f1WinningsCheck.Sum() / FlopTreeIterations,10:0.0000000} times the ante");
                Console.WriteLine($"Winnings raise 3:\t{f1WinningsRaise3.Sum() / FlopTreeIterations,10:0.0000000} times the ante");
                Console.WriteLine($"Winnings raise 4:\t{f1WinningsRaise4.Sum() / FlopTreeIterations,10:0.0000000} times the ante");
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