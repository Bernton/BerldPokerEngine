namespace BerldPokerEngine.Poker
{
    internal class HandValue : IComparable<HandValue>
    {
        internal int Hand { get; set; }
        internal int[] Ranks { get; }

        internal HandValue()
        {
            Hand = default;
            Ranks = new int[5];
        }

        public int CompareTo(HandValue? other)
        {
            if (other is null) return 1;

            int comparison = Hand - other.Hand;

            if (comparison == 0)
            {
                for (int ranksI = Ranks.Length - 1; ranksI >= 0; ranksI--)
                {
                    if (Ranks[ranksI] < 0) break;

                    comparison = Ranks[ranksI] - other.Ranks[ranksI];

                    if (comparison != 0) break;
                }
            }

            return comparison;
        }
    }
}
