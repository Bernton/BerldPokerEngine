namespace BerldPokerEngine.Poker
{
    internal struct HandValue
    {
        internal int Hand { get; set; }
        internal int[] Ranks { get; }

        public HandValue()
        {
            Hand = default;
            Ranks = new int[5];
        }
    }
}
