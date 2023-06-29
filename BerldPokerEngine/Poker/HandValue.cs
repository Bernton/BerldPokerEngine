namespace BerldPokerEngine.Poker
{
    public class HandValue : IComparable<HandValue>
    {
        public const int Amount = 5;
        public const int HandRankIndex = Amount - 1;

        public int Hand { get; set; }
        public int[] Ranks { get; }

        public HandValue()
        {
            Hand = default;
            Ranks = new int[Amount];
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
