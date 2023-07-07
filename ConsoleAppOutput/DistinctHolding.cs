using BerldPokerEngine.Poker;
using System.Text;

namespace ConsoleAppOutput
{
    internal class DistinctHolding : IComparable<DistinctHolding>
    {
        internal int Frequency { get; set; }
        internal string Key { get; }
        internal Card[] Cards { get; }

        internal DistinctHolding(Card[] cards, List<int>? markers)
        {
            Span<bool> wasAssigned = stackalloc bool[cards.Length];
            Card[] normalCards = new Card[cards.Length];

            int currentSuit = Suit.Clubs;

            for (int i = 0; i < cards.Length; i++)
            {
                if (wasAssigned[i]) continue;

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

            SortByMarkers(normalCards, markers);

            Card p1 = normalCards[0];
            Card p2 = normalCards[1];
            bool isPocketPair = p1.Rank == p2.Rank;

            if (isPocketPair)
            {
                var p1SuitGroup = normalCards.Where(c => c.Suit == p1.Suit).ToList();
                var p2SuitGroup = normalCards.Where(c => c.Suit == p2.Suit).ToList();

                if (p2SuitGroup.Count > p1SuitGroup.Count)
                {
                    for (int i = 2; i < normalCards.Length; i++)
                    {
                        if (normalCards[i].Suit == p1.Suit)
                        {
                            normalCards[i] = Card.Create(normalCards[i].Rank, p2.Suit);
                        }
                        else if (normalCards[i].Suit == p2.Suit)
                        {
                            normalCards[i] = Card.Create(normalCards[i].Rank, p1.Suit);
                        }
                    }
                }

                SortByMarkers(normalCards, markers);
            }

            Card f1 = normalCards[2];
            Card f2 = normalCards[3];

            if (f1.Rank == f2.Rank)
            {
                var f1SuitGroup = normalCards.Where(c => c.Suit == f1.Suit).ToList();
                var f2SuitGroup = normalCards.Where(c => c.Suit == f2.Suit).ToList();

                if (f2SuitGroup.Count > f1SuitGroup.Count)
                {
                    for (int i = 4; i < normalCards.Length; i++)
                    {
                        if (normalCards[i].Suit == f1.Suit)
                        {
                            normalCards[i] = Card.Create(normalCards[i].Rank, f2.Suit);
                        }
                        else if (normalCards[i].Suit == f2.Suit)
                        {
                            normalCards[i] = Card.Create(normalCards[i].Rank, f1.Suit);
                        }
                    }

                    SortByMarkers(normalCards, markers);
                }
            }

            Cards = normalCards;
            Key = CardsString(Cards);
        }

        private static void SortByMarkers(Card[] cards, List<int>? markers)
        {
            if (markers is null) return;

            for (int i = 0; i < markers.Count - 1; i++)
            {
                int first = markers[i];
                int last = markers[i + 1];
                int diff = last - first;

                if (first >= cards.Length) return;

                if (diff > 1)
                {
                    if (first + diff > cards.Length)
                    {
                        diff = cards.Length - first;
                    }

                    Array.Sort(cards, first, diff);
                }
            }
        }

        private static string GroupToString(List<(int rank, bool isPlayerCard)> group)
        {
            StringBuilder builder = new();

            for (int i = 0; i < group.Count; i++)
            {
                builder.Append(group[i].rank);
            }

            return builder.ToString();
        }

        private static string GetKey(List<List<(int rank, bool isPlayerCard)>> suitGroups)
        {
            int currentSuit = Suit.Clubs;
            StringBuilder builder = new();

            foreach (List<(int rank, bool isPlayerCard)> group in suitGroups)
            {
                foreach ((int rank, bool isPlayerCard) in group)
                {
                    Card card = Card.Create(rank, currentSuit);
                    builder.Append(card.ToString());
                }

                currentSuit++;
            }

            return builder.ToString();
        }

        public static string CardsString(Card[] cards)
        {
            StringBuilder builder = new();

            for (int i = 0; i < cards.Length; i++)
            {
                builder.Append(cards[i].ToString());
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return $"{CardsString(Cards)} {Frequency}";
        }

        public int CompareTo(DistinctHolding? other)
        {
            if (other is null) return 1;

            int comparison = 0;

            for (int i = 0; i < Cards.Length; i++)
            {
                comparison = Cards[i].Index - other.Cards[i].Index;

                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return comparison;
        }
    }
}
