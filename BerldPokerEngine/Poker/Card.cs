namespace BerldPokerEngine.Poker
{
    public readonly struct Card : IComparable<Card>
    {
        public int Rank { get; }
        public int Suit { get; }
        public int Index { get; }

        internal Card(int rank, int suit)
        {
            Rank = rank;
            Suit = suit;
            Index = rank * Poker.Suit.Amount + suit;
        }

        internal Card(int index)
        {
            Rank = index / Poker.Suit.Amount;
            Suit = index % Poker.Suit.Amount;
            Index = index;
        }

        public static Card Create(int rank, int suit)
        {
            if (rank < Poker.Rank.Deuce || rank > Poker.Rank.Ace)
            {
                throw new ArgumentException($"Invalid {nameof(rank)}.");
            }

            if (suit < Poker.Suit.Clubs || suit > Poker.Suit.Spades)
            {
                throw new ArgumentException($"Invalid {nameof(suit)}.");
            }

            return new Card(rank, suit);
        }

        public static Card Create(int index)
        {
            if (index < 0 || index >= Poker.Suit.Amount * Poker.Rank.Amount)
            {
                throw new ArgumentException($"Invalid {nameof(index)}.");
            }

            return new Card(index);
        }

        public override string ToString() => $"{Poker.Rank.ToChar(Rank)}{Poker.Suit.ToChar(Suit)}";

        public int CompareTo(Card other)
        {
            return Index - other.Index;
        }

        public static Card Card2c => new(Poker.Rank.Deuce, Poker.Suit.Clubs);
        public static Card Card2d => new(Poker.Rank.Deuce, Poker.Suit.Diamonds);
        public static Card Card2h => new(Poker.Rank.Deuce, Poker.Suit.Hearts);
        public static Card Card2s => new(Poker.Rank.Deuce, Poker.Suit.Spades);

        public static Card Card3c => new(Poker.Rank.Tray, Poker.Suit.Clubs);
        public static Card Card3d => new(Poker.Rank.Tray, Poker.Suit.Diamonds);
        public static Card Card3h => new(Poker.Rank.Tray, Poker.Suit.Hearts);
        public static Card Card3s => new(Poker.Rank.Tray, Poker.Suit.Spades);

        public static Card Card4c => new(Poker.Rank.Four, Poker.Suit.Clubs);
        public static Card Card4d => new(Poker.Rank.Four, Poker.Suit.Diamonds);
        public static Card Card4h => new(Poker.Rank.Four, Poker.Suit.Hearts);
        public static Card Card4s => new(Poker.Rank.Four, Poker.Suit.Spades);

        public static Card Card5c => new(Poker.Rank.Five, Poker.Suit.Clubs);
        public static Card Card5d => new(Poker.Rank.Five, Poker.Suit.Diamonds);
        public static Card Card5h => new(Poker.Rank.Five, Poker.Suit.Hearts);
        public static Card Card5s => new(Poker.Rank.Five, Poker.Suit.Spades);

        public static Card Card6c => new(Poker.Rank.Six, Poker.Suit.Clubs);
        public static Card Card6d => new(Poker.Rank.Six, Poker.Suit.Diamonds);
        public static Card Card6h => new(Poker.Rank.Six, Poker.Suit.Hearts);
        public static Card Card6s => new(Poker.Rank.Six, Poker.Suit.Spades);

        public static Card Card7c => new(Poker.Rank.Seven, Poker.Suit.Clubs);
        public static Card Card7d => new(Poker.Rank.Seven, Poker.Suit.Diamonds);
        public static Card Card7h => new(Poker.Rank.Seven, Poker.Suit.Hearts);
        public static Card Card7s => new(Poker.Rank.Seven, Poker.Suit.Spades);

        public static Card Card8c => new(Poker.Rank.Eight, Poker.Suit.Clubs);
        public static Card Card8d => new(Poker.Rank.Eight, Poker.Suit.Diamonds);
        public static Card Card8h => new(Poker.Rank.Eight, Poker.Suit.Hearts);
        public static Card Card8s => new(Poker.Rank.Eight, Poker.Suit.Spades);

        public static Card Card9c => new(Poker.Rank.Nine, Poker.Suit.Clubs);
        public static Card Card9d => new(Poker.Rank.Nine, Poker.Suit.Diamonds);
        public static Card Card9h => new(Poker.Rank.Nine, Poker.Suit.Hearts);
        public static Card Card9s => new(Poker.Rank.Nine, Poker.Suit.Spades);

        public static Card CardTc => new(Poker.Rank.Ten, Poker.Suit.Clubs);
        public static Card CardTd => new(Poker.Rank.Ten, Poker.Suit.Diamonds);
        public static Card CardTh => new(Poker.Rank.Ten, Poker.Suit.Hearts);
        public static Card CardTs => new(Poker.Rank.Ten, Poker.Suit.Spades);

        public static Card CardJc => new(Poker.Rank.Jack, Poker.Suit.Clubs);
        public static Card CardJd => new(Poker.Rank.Jack, Poker.Suit.Diamonds);
        public static Card CardJh => new(Poker.Rank.Jack, Poker.Suit.Hearts);
        public static Card CardJs => new(Poker.Rank.Jack, Poker.Suit.Spades);

        public static Card CardQc => new(Poker.Rank.Queen, Poker.Suit.Clubs);
        public static Card CardQd => new(Poker.Rank.Queen, Poker.Suit.Diamonds);
        public static Card CardQh => new(Poker.Rank.Queen, Poker.Suit.Hearts);
        public static Card CardQs => new(Poker.Rank.Queen, Poker.Suit.Spades);

        public static Card CardKc => new(Poker.Rank.King, Poker.Suit.Clubs);
        public static Card CardKd => new(Poker.Rank.King, Poker.Suit.Diamonds);
        public static Card CardKh => new(Poker.Rank.King, Poker.Suit.Hearts);
        public static Card CardKs => new(Poker.Rank.King, Poker.Suit.Spades);

        public static Card CardAc => new(Poker.Rank.Ace, Poker.Suit.Clubs);
        public static Card CardAd => new(Poker.Rank.Ace, Poker.Suit.Diamonds);
        public static Card CardAh => new(Poker.Rank.Ace, Poker.Suit.Hearts);
        public static Card CardAs => new(Poker.Rank.Ace, Poker.Suit.Spades);
    }
}
