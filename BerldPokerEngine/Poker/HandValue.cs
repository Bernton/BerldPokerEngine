namespace BerldPokerEngine.Poker
{
    internal struct HandValue
    {
        internal int hand;
        internal int rank0;
        internal int rank1;
        internal int rank2;
        internal int rank3;
        internal int rank4;

        public HandValue()
        {
            hand = -1;
            rank0 = -1;
            rank1 = -1;
            rank2 = -1;
            rank3 = -1;
            rank4 = -1;
        }
    }
}
