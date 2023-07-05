using BerldPokerEngine.Poker;
using BerldPokerEngine;

namespace ConsoleAppOutput
{
    internal static class DistinctHoldingCalculator
    {
        internal static void Start()
        {
            List<Card> cards = EngineData.GetAllCards();

            Dictionary<string, DistinctHolding> holdingMap = new();

            Card[] holdingCards = new Card[5];

            for (int c0 = 0; c0 < cards.Count; c0++)
            {
                holdingCards[0] = cards[c0];

                for (int c1 = c0 + 1; c1 < cards.Count; c1++)
                {
                    holdingCards[1] = cards[c1];

                    for (int c2 = c1 + 1; c2 < cards.Count; c2++)
                    {
                        holdingCards[2] = cards[c2];

                        for (int c3 = c2 + 1; c3 < cards.Count; c3++)
                        {
                            holdingCards[3] = cards[c3];

                            for (int c4 = c3 + 1; c4 < cards.Count; c4++)
                            {
                                holdingCards[4] = cards[c4];


                                DistinctHolding holding = new(holdingCards);

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
                }
            }

            DistinctHolding[] holdings = holdingMap.Values.ToArray();



            int totalFrequency = holdings.Sum(c => c.Frequency);

            Console.WriteLine($"Amount of distinct holdings: {holdings.Length} (should be 134459)");
            Console.WriteLine($"Total frequency: {totalFrequency}");
        }
    }
}
