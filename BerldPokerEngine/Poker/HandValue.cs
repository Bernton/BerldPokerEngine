namespace BerldPokerEngine.Poker
{
    internal struct HandValue
    {
        internal int hand;
        internal int[] ranks;

        public HandValue()
        {
            hand = -1;
            ranks = new[] { -1, -1, -1, -1, -1 };
        }
    }
}
