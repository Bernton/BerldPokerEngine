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
        }
    }
}
