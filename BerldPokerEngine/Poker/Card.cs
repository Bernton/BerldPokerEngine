namespace BerldPokerEngine.Poker
{
    internal struct Card
    {
        internal int rank;
        internal int suit;

        internal Card(int rank, int suit)
        {
            this.rank = rank;
            this.suit = suit;
        }

        internal Card(int index)
        {
            rank = index / 4;
            suit = index % 4;
        }
    }
}
