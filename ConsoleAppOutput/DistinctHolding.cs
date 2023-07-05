using BerldPokerEngine.Poker;
using System.Text;

namespace ConsoleAppOutput
{
    internal class DistinctHolding
    {
        internal Card[] Cards { get; }
        internal string Key { get; }
        internal int Frequency { get; set; }

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
            Key = GetKey(normalCards);
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
