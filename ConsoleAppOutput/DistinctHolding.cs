using BerldPokerEngine.Poker;
using System.Text;

namespace ConsoleAppOutput
{
    internal class DistinctHolding : IComparable<DistinctHolding>
    {
        internal int Frequency { get; set; }
        internal string Key { get; }
        internal Card[] Cards { get; }
        internal List<List<(int rank, bool isPlayerCard)>> SuitGroups { get; } = new();


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
            Cards = normalCards;

            bool useSuitGroup = true;
            Card p1 = cards[0];
            Card p2 = cards[1];

            for (int i = 0; i < Suit.Amount; i++)
            {
                List<(int rank, bool isPlayerCard)> suitGroup =
                    normalCards
                        .Where(c => c.Suit == i)
                        .Select(c => (c.Rank, c.Index == p1.Index || c.Index == p1.Index))
                        .ToList();

                SuitGroups.Add(suitGroup);
            }

            if (cards[0].Suit == cards[1].Suit)
            {
                SuitGroups =
                    SuitGroups
                        .OrderByDescending(c => c.Count(c => c.isPlayerCard))
                        .ThenByDescending(c => c.Count)
                        .ThenBy(GroupToString)
                        .ToList();
            }
            else
            {
                useSuitGroup = false;
                //List<(int, bool)>? p1Group = SuitGroups.Find(c => c.Any(c => c.Item2 && c.Item1 == cards[0].Rank));
                //List<(int, bool)>? p2Group = SuitGroups.Find(c => c.Any(c => c.Item2 && c.Item1 == cards[0].Rank));
            }

            Key = useSuitGroup ? GetKey(SuitGroups) : CardsString();
            //Key = CardsString();
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

        public string CardsString()
        {
            StringBuilder builder = new();

            for (int i = 0; i < Cards.Length; i++)
            {
                builder.Append(Cards[i].ToString());
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return $"{CardsString()} {Frequency}";
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
