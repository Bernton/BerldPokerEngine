using BerldPokerEngine.Poker;
using System.Diagnostics;
using System.Text;

namespace CasinoHoldemSimulator
{
    internal class NormalRound
    {
        internal List<Card> PlayerCards { get; private set; }
        internal List<Card> FlopCards { get; private set; }
        internal List<Card> Cards { get; private set; }
        internal string Key { get; private set; }
        internal int Frequency { get; set; }

        internal NormalRound(int p1, int p2, int f1, int f2, int f3)
        {
            Frequency = 1;
            List<Card> playerCards = GetPlayerCards(p1, p2);
            List<Card> flopCards = GetFlopCards(f1, f2, f3);

            // Normalize
            playerCards = playerCards.OrderBy(c => c.Index).ToList();
            flopCards = flopCards.OrderBy(c => c.Index).ToList();
            List<Card> cards = Enumerable.Concat(playerCards, flopCards).ToList();
            Card?[] normalCards = new Card?[cards.Count];

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

            List<Card> normalPlayerCards = new();

            for (int i = 0; i < RoundEngine.PlayerCardAmount; i++)
            {
                Card? normalCard = normalCards[i];
                Debug.Assert(normalCard.HasValue);
                normalPlayerCards.Add(normalCard.Value);
            }

            List<Card> normalFlopCards = new();

            for (int i = 0; i < RoundEngine.FlopCardAmount; i++)
            {
                Card? normalCard = normalCards[RoundEngine.PlayerCardAmount + i];
                Debug.Assert(normalCard.HasValue);
                normalFlopCards.Add(normalCard.Value);
            }

            normalPlayerCards = normalPlayerCards.OrderBy(c => c.Index).ToList();
            normalFlopCards = normalFlopCards.OrderBy(c => c.Index).ToList();

            PlayerCards = normalPlayerCards;
            FlopCards = normalFlopCards;
            Cards = Enumerable.Concat(PlayerCards, FlopCards).ToList();
            Key = GetKey();
        }

        private static List<Card> GetPlayerCards(int p1, int p2)
        {
            return new()
            {
                Card.Create(p1),
                Card.Create(p2)
            };
        }

        private static List<Card> GetFlopCards(int f1, int f2, int f3)
        {
            return new()
            {
                Card.Create(f1),
                Card.Create(f2),
                Card.Create(f3)
            };
        }

        private string GetKey()
        {
            StringBuilder builder = new();

            foreach (Card card in Cards)
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
