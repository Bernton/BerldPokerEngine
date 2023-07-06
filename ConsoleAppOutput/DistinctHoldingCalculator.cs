using BerldPokerEngine.Poker;
using BerldPokerEngine;
using System.Text;

namespace ConsoleAppOutput
{
    internal static class DistinctHoldingCalculator
    {
        internal static void Start()
        {
            const int CardAmount = 7;

            List<Card> cards = EngineData.GetAllCards();

            Dictionary<string, DistinctHolding> holdingMap = new();

            Card[] holdingCards = new Card[CardAmount];

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


                                for (int c5 = c4 + 1; c5 < cards.Count; c5++)
                                {
                                    holdingCards[5] = cards[c5];

                                    for (int c6 = c5 + 1; c6 < cards.Count; c6++)
                                    {
                                        holdingCards[6] = cards[c6];

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
                }
            }

            DistinctHolding[] holdings = holdingMap.Values.OrderBy(c => c).ToArray();

            int totalFrequency = holdings.Sum(c => c.Frequency);

            Console.WriteLine($"Amount of distinct holdings: {holdings.Length}");
            Console.WriteLine($"Total frequency: {totalFrequency}");
            Console.WriteLine();

            string fileName = $"distinct{CardAmount}Holdings.csv";
            StringBuilder fileContentBuilder = new();

            for (int i = 0; i < holdings.Length; i++)
            {
                DistinctHolding holding = holdings[i];
                fileContentBuilder.Append(holding.CardsString());
                fileContentBuilder.Append(',');
                fileContentBuilder.Append(holding.Frequency);
                fileContentBuilder.AppendLine();
            }

            File.WriteAllText(fileName, fileContentBuilder.ToString());
            Console.WriteLine($"Wrote file '{fileName}'");
            Console.WriteLine();

            HandValue value = new();
            int[] handAmounts = new int[Hand.Amount];

            foreach (DistinctHolding holding in holdings)
            {
                Engine.SetHandValue(holding.Cards, value);
                handAmounts[value.Hand] += holding.Frequency;
            }

            for (int hand = 0; hand < Hand.Amount; hand++)
            {
                string formattedHand = Hand.ToFormatString(hand);
                string tabPadding = Hand.GetTabPadding(hand);
                int handAmount = handAmounts[hand];
                Console.WriteLine($"{formattedHand}:{tabPadding}{handAmount,15}");
            }
        }
    }
}
