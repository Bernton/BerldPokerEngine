using BerldPokerEngine.Poker;
using System.Text;

namespace TexasHoldemBonusSimulator
{
    internal class DistinctHolding
    {
        internal Card[] Cards { get; }
        internal string Key { get; }
        internal int Frequency { get; set; }

        internal DistinctHolding(Card[] cards, List<int> sortMarkers)
        {
            // SortByMarkers(cards, sortMarkers);

            bool[] wasAssigned = new bool[cards.Length];
            Card[] normalCards = new Card[cards.Length];

            int currentSuit = Suit.Clubs;

            for (int i = 0; i < cards.Length; i++)
            {
                if (wasAssigned[i])
                {
                    continue;
                }

                int suitToMap = cards[i].Suit;

                for (int j = i; j < cards.Length; j++)
                {
                    if (cards[j].Suit == suitToMap)
                    {
                        wasAssigned[j] = true;
                        normalCards[j] = Card.Create(cards[j].Rank, currentSuit);
                    }
                }

                currentSuit++;
            }

            SortByMarkers(normalCards, sortMarkers);

            Cards = normalCards;
            Key = GetKey(normalCards);
        }

        private static void SortByMarkers(Card[] cards, List<int> markers)
        {
            for (int i = 0; i < markers.Count - 1; i++)
            {
                int first = markers[i];
                int last = markers[i + 1];
                int diff = last - first;

                if (diff > 1)
                {
                    Array.Sort(cards, first, diff);
                }
            }
        }

        private static string GetKey(Card[] cards)
        {
            StringBuilder builder = new();

            foreach (Card card in cards)
            {
                builder.Append(card.ToString());
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return $"{Key} {Frequency}";
        }
    }
}
