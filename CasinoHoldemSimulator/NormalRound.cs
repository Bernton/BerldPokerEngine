﻿using BerldPokerEngine.Poker;
using System.Diagnostics;
using System.Text;

namespace CasinoHoldemSimulator
{
    internal class NormalRound
    {
        public List<Card> PlayerCards { get; private set; }
        public List<Card> FlopCards { get; private set; }
        public List<Card> Cards { get; private set; }
        public string Key { get; private set; }
        public int Frequency { get; set; }

        public NormalRound(int p1, int p2, int b1, int b2, int b3)
        {
            Frequency = 1;
            List<Card> playerCards = GetPlayerCards(p1, p2);
            List<Card> flopCards = GetFlopCards(b1, b2, b3);

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

        private static List<Card> GetFlopCards(int b1, int b2, int b3)
        {
            return new()
            {
                Card.Create(b1),
                Card.Create(b2),
                Card.Create(b3)
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