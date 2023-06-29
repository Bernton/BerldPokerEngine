using BerldPokerEngine.Poker;
using System.Diagnostics;
using System.Text;

namespace CasinoHoldemSimulator
{
    internal class NormalRound
    {
        private const int CardAmount = RoundEngine.PlayerCardAmount + RoundEngine.FlopCardAmount;

        internal List<Card> PlayerCards { get; private set; }
        internal List<Card> FlopCards { get; private set; }
        internal string Key { get; private set; }
        internal int Frequency { get; set; } = 1;

        internal NormalRound(int p1, int p2, int f1, int f2, int f3)
        {
            // Normalize
            List<Card> cards = GetCards(p1, p2, f1, f2, f3);
            Card?[] normalCards = new Card?[CardAmount];

            int currentSuit = Suit.Clubs;

            for (int i = 0; i < cards.Count; i++)
            {
                if (normalCards[i].HasValue)
                {
                    continue;
                }

                int suitToMap = cards[i].Suit;

                for (int j = i; j < cards.Count; j++)
                {
                    if (cards[j].Suit == suitToMap)
                    {
                        normalCards[j] = Card.Create(cards[j].Rank, currentSuit);
                    }
                }

                currentSuit++;
            }

            List<Card> normalPlayerCards = new(RoundEngine.PlayerCardAmount);

            for (int i = 0; i < RoundEngine.PlayerCardAmount; i++)
            {
                Card? normalCard = normalCards[i];
                Debug.Assert(normalCard.HasValue);
                normalPlayerCards.Add(normalCard.Value);
            }

            List<Card> normalFlopCards = new(RoundEngine.FlopCardAmount);

            for (int i = 0; i < RoundEngine.FlopCardAmount; i++)
            {
                Card? normalCard = normalCards[RoundEngine.PlayerCardAmount + i];
                Debug.Assert(normalCard.HasValue);
                normalFlopCards.Add(normalCard.Value);
            }

            normalPlayerCards.Sort();
            normalFlopCards.Sort();

            PlayerCards = normalPlayerCards;
            FlopCards = normalFlopCards;
            Key = GetKey();
        }

        private static List<Card> GetCards(int p1, int p2, int f1, int f2, int f3)
        {
            return new(CardAmount)
            {
                Card.Create(p1),
                Card.Create(p2),
                Card.Create(f1),
                Card.Create(f2),
                Card.Create(f3)
            };
        }

        private string GetKey()
        {
            StringBuilder builder = new();

            foreach (Card card in PlayerCards)
            {
                builder.Append(card.ToString());
            }

            foreach (Card card in FlopCards)
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
