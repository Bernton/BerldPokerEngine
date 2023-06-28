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

        public Deck()
        {
            Reset();
        }

        public void Reset()
        {
            ResetCards();
            _cardsDrawn = 0;
        }

        public Card Draw()
        {
            int chosenIndex = _random.Next(AllCardsAmount - _cardsDrawn);
            Card drawnCard = _cards[chosenIndex];
            _cards[chosenIndex] = _cards[LastCardIndex - _cardsDrawn];
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
