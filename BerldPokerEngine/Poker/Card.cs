namespace BerldPokerEngine.Poker
{
    internal readonly struct Card
    {
        internal int Rank { get; }
        internal int Suit { get; }

        internal Card(int rank, int suit)
        {
            Rank = rank;
            Suit = suit;
        }

        internal Card(int index)
        {
            Rank = index / Poker.Suit.Amount;
            Suit = index % Poker.Suit.Amount;
        }

        public override string ToString()
        {
            return $"{Poker.Rank.ToChar(Rank)}{Poker.Suit.ToChar(Suit)}";
        }
    }
}
