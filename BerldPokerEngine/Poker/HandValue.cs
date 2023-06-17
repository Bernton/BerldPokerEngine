namespace BerldPokerEngine.Poker
{
    internal class HandValue
    {
        internal int Hand { get; set; }
        internal int[] Ranks { get; }

        internal HandValue()
        {
            Hand = default;
            Ranks = new int[5];
        }
    }
}
