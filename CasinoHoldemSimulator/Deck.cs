using BerldPokerEngine.Poker;

namespace CasinoHoldemSimulator
{
    internal class Deck
    {
        private const int AllCardsAmount = 52;
        private const int LastCardIndex = AllCardsAmount - 1;

        private readonly Random _random = new();
        private readonly Card[] _cards = new Card[AllCardsAmount];
        private int _cardsDrawn = 0;

        private readonly Card[] _snapshotCards = new Card[AllCardsAmount];
        private int _snapshotCardsDrawn = 0;

        public Deck()
        {
            Reset();
        }

        public void Reset()
        {
            ResetCards();
            _cardsDrawn = 0;
        }

        public void Snapshot()
        {
            for (int i = 0; i < AllCardsAmount; i++)
            {
                _snapshotCards[i] = _cards[i];
                _snapshotCardsDrawn = _cardsDrawn;
            }
        }

        public void Restore()
        {
            for (int i = 0; i < AllCardsAmount; i++)
            {
                _cards[i] = _snapshotCards[i];
                _cardsDrawn = _snapshotCardsDrawn;
            }
        }

        public Card Draw()
        {
            int chosenIndex = _random.Next(AllCardsAmount - _cardsDrawn);
            return Draw(chosenIndex);
        }

        public Card Draw(Card card)
        {
            int cardsLeft = AllCardsAmount - _cardsDrawn;

            for (int i = 0; i < cardsLeft; i++)
            {
                if (card.Index == _cards[i].Index)
                {
                    return Draw(i);
                }
            }

            throw new ArgumentException("Card not in the deck.");
        }

        public Card Draw(int index)
        {
            Card drawnCard = _cards[index];
            _cards[index] = _cards[LastCardIndex - _cardsDrawn];
            _cardsDrawn++;
            return drawnCard;
        }

        private void ResetCards()
        {
            for (int i = 0; i < AllCardsAmount; i++)
            {
                _cards[i] = Card.Create(i);
            }
        }
    }
}
