namespace BerldPokerEngine.Poker
{
    internal class HandValue
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
