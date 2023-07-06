using BerldPokerEngine.Poker;
using System.Text;

namespace ConsoleAppOutput
{
    internal class DistinctHolding : IComparable<DistinctHolding>
    {
        internal int Frequency { get; set; }
        internal string Key { get; }
        internal Card[] Cards { get; }
        internal List<List<int>> SuitGroups { get; } = new();


        internal DistinctHolding(Card[] cards)
        {
            Span<bool> wasAssigned = stackalloc bool[cards.Length];
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

            Array.Sort(normalCards);
            Cards = normalCards;

            for (int i = 0; i < Suit.Amount; i++)
            {
                List<int> suitGroup = normalCards.Where(c => c.Suit == i).Select(c => c.Rank).ToList();
                SuitGroups.Add(suitGroup);
            }

            SuitGroups = SuitGroups.OrderByDescending(c => c.Count).ThenBy(GroupToString).ToList();
            Key = GetKey(SuitGroups);
        }

        private static string GroupToString(List<int> group)
        {
            StringBuilder builder = new();

            for (int i = 0; i < group.Count; i++)
            {
                builder.Append(group[i]);
            }

            return builder.ToString();
        }

        private static string GetKey(List<List<int>> suitGroups)
        {
            StringBuilder builder = new();

            int currentSuit = Suit.Clubs;

            foreach (List<int> group in suitGroups)
            {
                foreach (int rank in group)
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
