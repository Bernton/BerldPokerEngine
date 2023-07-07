using BerldPokerEngine;
using BerldPokerEngine.Poker;

namespace UltimateTexasHoldemSimulator
{
    internal class Program
    {
        private const int Ante = 1;
        private const int Blind = Ante;

        const long FlopIterations = 20975724000;
        const int RiverIterations = 1070190;
        const int ResultIterations = 990;

        const int FlopOnly = 19600;

        private static void Main(string[] args)
        {
            // Ante 1, Blind 1, Play 0

            // See first 2 cards

            // Check, raise 4x or raise 3x

            List<Card> playerCards = new() { Card.Card8c, Card.CardJc };

            //(double winnings3, double winnings4) = OnRaise3or4(playerCards);

            double winnings2 = EvaluateFlop(playerCards);
            Console.WriteLine(winnings2 / FlopIterations);
            //Console.WriteLine(winnings3);
            //Console.WriteLine(winnings4);
        }


        private static (double winnings, int playBetWins) EvaluateResult(List<Card> playerCards, Card[] boardCards)
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
            int playBetWins = 0;

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

        private static (double raiseFlopWinnings, double checkFlopWinnings) EvaluateRiver(List<Card> playerCards, Card[] flopCards)
        {
            const int TurnRaise = 2 * Ante;
            const int RiverRaise = 1 * Ante;

            const int FoldWinnings = -(Ante + Blind) * ResultIterations;

            List<Card> aliveCards = EngineData.GetAllCards().Except(playerCards).Except(flopCards).ToList();

            Card[] boardCards = new Card[5];

            for (int i = 0; i < flopCards.Length; i++)
            {
                boardCards[i] = flopCards[i];
            }

            double raiseFlopWinnings = 0;
            double checkFlopWinnings = 0;

            for (int b4 = 0; b4 < aliveCards.Count; b4++)
            {
                boardCards[3] = aliveCards[b4];

                for (int b5 = b4 + 1; b5 < aliveCards.Count; b5++)
                {
                    boardCards[4] = aliveCards[b5];

                    (double winnings, int playBetWins) = EvaluateResult(playerCards, boardCards);

                    raiseFlopWinnings += winnings + playBetWins * TurnRaise;

                    double raiseRiverWinnings = winnings + playBetWins * RiverRaise;
                    checkFlopWinnings += Math.Max(raiseRiverWinnings, FoldWinnings);
                }
            }

            return (raiseFlopWinnings, checkFlopWinnings);
        }

        private static double EvaluateFlop(List<Card> playerCards)
        {
            List<Card> aliveCards = EngineData.GetAllCards().Except(playerCards).ToList();

            Card[] flopCards = new Card[3];

            double winnings = 0;
            int count = 0;

            for (int f1 = 0; f1 < aliveCards.Count; f1++)
            {
                flopCards[0] = aliveCards[f1];

                for (int f2 = f1 + 1; f2 < aliveCards.Count; f2++)
                {
                    flopCards[1] = aliveCards[f2];

                    for (int f3 = f2 + 1; f3 < aliveCards.Count; f3++)
                    {
                        flopCards[2] = aliveCards[f3];

                        (double raiseFlopWinnings, double checkFlopWinnings) = EvaluateRiver(playerCards, flopCards);

                        double flopWinnings;

                        if (raiseFlopWinnings > checkFlopWinnings)
                        {
                            flopWinnings = raiseFlopWinnings;
                        }
                        else
                        {
                            flopWinnings = checkFlopWinnings;
                        }

                        winnings += flopWinnings;

                        count++;

                        if (count % 25 == 0)
                        {
                            Console.WriteLine($"Progress: {count / (double)FlopOnly * 100:0.00}%");
                            Console.WriteLine($"Relative winnings: {winnings / FlopIterations:0.0000000}");
                            Console.WriteLine();
                        }
                    }
                }
            }

            return winnings;
        }

        private static (double winnings3, double winnings4) OnRaise3or4(List<Card> playerCards)
        {
            const int Play3 = Ante * 3;
            const int Play4 = Ante * 4;

            List<Card> aliveCards = EngineData.GetAllCards().Except(playerCards).ToList();
            double[] b1Winnings = new double[aliveCards.Count];
            double[] b1PlayNet = new double[aliveCards.Count];

            Parallel.For(0, aliveCards.Count, b1 =>
            {
                HandValue playerValue = new();
                Card[] playerCardsToEvaluate = new Card[7];

                playerCardsToEvaluate[5] = playerCards[0];
                playerCardsToEvaluate[6] = playerCards[1];

                HandValue dealerValue = new();
                Card[] dealerCardsToEvaluate = new Card[7];

                playerCardsToEvaluate[0] = aliveCards[b1];
                dealerCardsToEvaluate[0] = aliveCards[b1];

                for (int b2 = b1 + 1; b2 < aliveCards.Count; b2++)
                {
                    playerCardsToEvaluate[1] = aliveCards[b2];
                    dealerCardsToEvaluate[1] = aliveCards[b2];

                    for (int b3 = b2 + 1; b3 < aliveCards.Count; b3++)
                    {
                        playerCardsToEvaluate[2] = aliveCards[b3];
                        dealerCardsToEvaluate[2] = aliveCards[b3];

                        for (int b4 = b3 + 1; b4 < aliveCards.Count; b4++)
                        {
                            playerCardsToEvaluate[3] = aliveCards[b4];
                            dealerCardsToEvaluate[3] = aliveCards[b4];

                            for (int b5 = b4 + 1; b5 < aliveCards.Count; b5++)
                            {
                                playerCardsToEvaluate[4] = aliveCards[b5];
                                dealerCardsToEvaluate[4] = aliveCards[b5];

                                for (int d1 = 0; d1 < aliveCards.Count; d1++)
                                {
                                    if (d1 == b1 || d1 == b2 || d1 == b3 || d1 == b4 || d1 == b5) continue;
                                    dealerCardsToEvaluate[5] = aliveCards[d1];

                                    for (int d2 = d1 + 1; d2 < aliveCards.Count; d2++)
                                    {
                                        if (d2 == b1 || d2 == b2 || d2 == b3 || d2 == b4 || d2 == b5) continue;
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
                                                b1Winnings[b1] += Ante;
                                            }

                                            b1PlayNet[b1] += 1;
                                            b1Winnings[b1] += Blind * GetBlindMuliplier(playerValue.Hand);
                                        }
                                        else if (dealerWins)
                                        {
                                            if (dealerQualified)
                                            {
                                                b1Winnings[b1] -= Ante;
                                            }

                                            b1PlayNet[b1] -= 1;
                                            b1Winnings[b1] -= Blind;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            double winnings = b1Winnings.Sum();
            double playNet = b1PlayNet.Sum();

            double winnings3 = winnings + playNet * Play3;
            double winnings4 = winnings + playNet * Play4;

            return (winnings3, winnings4);
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