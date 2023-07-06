using BerldPokerEngine;
using BerldPokerEngine.Poker;

namespace ConsoleAppOutput
{
    internal static class DistinctHoldingCalculator
    {
        internal static void Start()
        {
            const int CardAmount = 5;
            List<int> sectionMarkers = new() { 0, 2, CardAmount };

            List<Card> allCards = EngineData.GetAllCards();
            Card[] playerCards = new Card[CardAmount];

            Dictionary<string, DistinctHolding> holdingMap = new();

            for (int p1 = 0; p1 < allCards.Count; p1++)
            {
                playerCards[0] = allCards[p1];

                for (int p2 = p1 + 1; p2 < allCards.Count; p2++)
                {
                    playerCards[1] = allCards[p2];

                    for (int f1 = 0; f1 < allCards.Count; f1++)
                    {
                        if (f1 == p1 || f1 == p2) continue;
                        playerCards[2] = allCards[f1];

                        for (int f2 = f1 + 1; f2 < allCards.Count; f2++)
                        {
                            if (f2 == p1 || f2 == p2) continue;
                            playerCards[3] = allCards[f2];

                            for (int f3 = f2 + 1; f3 < allCards.Count; f3++)
                            {
                                if (f3 == p1 || f3 == p2) continue;
                                playerCards[4] = allCards[f3];

                                DistinctHolding holding = new(playerCards, sectionMarkers);

                                if (holdingMap.ContainsKey(holding.Key))
                                {
                                    DistinctHolding existingHolding = holdingMap[holding.Key];
                                    existingHolding.Frequency += 1;
                                }
                                else
                                {
                                    holding.Frequency = 1;
                                    holdingMap.Add(holding.Key, holding);
                                }
                            }
                        }
                    }
                }
            }

            DistinctHolding[] holdings = holdingMap.Values.ToArray();

            Console.WriteLine($"Amount of distinct holdings: {holdings.Length}");
            Console.WriteLine();

            HandValue handValue = new();
            int[] handAmounts = new int[Hand.Amount];

            for (int i = 0; i < holdings.Length; i++)
            {
                DistinctHolding holding = holdings[i];
                Engine.SetHandValue(holding.Cards, handValue);
                handAmounts[handValue.Hand] += holding.Frequency;
            }

            Console.WriteLine($"All:\t\t\t{handAmounts.Sum(c => c),15}");

            for (int hand = 0; hand < Hand.Amount; hand++)
            {
                string formattedHand = Hand.ToFormatString(hand);
                string tabPadding = Hand.GetTabPadding(hand);
                int handAmount = handAmounts[hand];
                Console.WriteLine($"{formattedHand}:{tabPadding}{handAmount,15}");
            }

            Console.WriteLine();

            int pair = 0;
            int twoPair = 0;
            int trips = 0;
            int sets = 0;
            int quads_2_2 = 0;
            int quads_1_3 = 0;
            int flushDraws_1_2 = 0;
            int flushDraws_2_1 = 0;
            int flushDraws_2_2 = 0;
            int flushDraws_1_3 = 0;
            int flushes = 0;

            for (int holdingI = 0; holdingI < holdings.Length; holdingI++)
            {
                DistinctHolding holding = holdings[holdingI];

                Card p1 = holding.Cards[0];
                Card p2 = holding.Cards[1];

                Card[] flop = new Card[]
                {
                    holding.Cards[2],
                    holding.Cards[3],
                    holding.Cards[4]
                };

                int[] rankAmounts = new int[Rank.Amount];

                for (int i = 0; i < flop.Length; i++)
                {
                    rankAmounts[flop[i].Rank]++;
                }

                if (p1.Rank == p2.Rank)
                {
                    if (rankAmounts[p1.Rank] == 2)
                    {
                        quads_2_2 += holding.Frequency;
                    }
                    else if (rankAmounts[p1.Rank] == 1)
                    {
                        sets += holding.Frequency;
                    }
                }
                else
                {
                    if (rankAmounts[p1.Rank] == 3 || rankAmounts[p2.Rank] == 3)
                    {
                        quads_1_3 += holding.Frequency;
                    }
                    else if (rankAmounts[p1.Rank] == 2 || rankAmounts[p2.Rank] == 2)
                    {
                        trips += holding.Frequency;
                    }
                    else if (rankAmounts[p1.Rank] == 1 && rankAmounts[p2.Rank] == 1)
                    {
                        twoPair += holding.Frequency;
                    }
                    else if (rankAmounts[p1.Rank] == 1 || rankAmounts[p2.Rank] == 1)
                    {
                        pair += holding.Frequency;
                    }
                }

                int[] suitAmounts = new int[Suit.Amount];

                for (int i = 0; i < flop.Length; i++)
                {
                    suitAmounts[flop[i].Suit]++;
                }

                if (p1.Suit == p2.Suit && suitAmounts[p1.Suit] == 1)
                {
                    flushDraws_2_1 += holding.Frequency;
                }

                int? flushFlop3 = null;

                for (int i = 0; i < Suit.Amount; i++)
                {
                    if (suitAmounts[i] == 3)
                    {
                        flushFlop3 = i;
                        break;
                    }
                }

                if (flushFlop3.HasValue)
                {
                    if (p1.Suit == flushFlop3.Value && p2.Suit == flushFlop3.Value)
                    {
                        flushes += holding.Frequency;
                    }
                    else if (p1.Suit == flushFlop3.Value || p2.Suit == flushFlop3.Value)
                    {
                        flushDraws_1_3 += holding.Frequency;
                    }
                }

                int? flushFlop2 = null;

                for (int i = 0; i < Suit.Amount; i++)
                {
                    if (suitAmounts[i] == 2)
                    {
                        flushFlop2 = i;
                        break;
                    }
                }

                if (flushFlop2.HasValue)
                {
                    if (p1.Suit == flushFlop2.Value &&
                        p2.Suit == flushFlop2.Value)
                    {
                        flushDraws_2_2 += holding.Frequency;
                    }
                    else if (p1.Suit == flushFlop2.Value ||
                        p2.Suit == flushFlop2.Value)
                    {
                        flushDraws_1_2 += holding.Frequency;
                    }
                }
            }

            Console.WriteLine($"Pair:\t\t\t{pair,10}");
            Console.WriteLine($"Two pair:\t\t{twoPair,10}");
            Console.WriteLine($"Trips:\t\t\t{trips,10}");
            Console.WriteLine($"Sets:\t\t\t{sets,10}");
            Console.WriteLine($"Quads 2 2:\t\t{quads_2_2,10}");
            Console.WriteLine($"Quads 1 3:\t\t{quads_1_3,10}");
            Console.WriteLine($"Flush draws 1 2:\t{flushDraws_1_2,10}");
            Console.WriteLine($"Flush draws 2 1:\t{flushDraws_2_1,10}");
            Console.WriteLine($"Flush draws 2 2:\t{flushDraws_2_2,10}");
            Console.WriteLine($"Flush draws 1 3:\t{flushDraws_1_3,10}");
            Console.WriteLine($"Flushes:\t\t{flushes,10}");
        }
    }
}
