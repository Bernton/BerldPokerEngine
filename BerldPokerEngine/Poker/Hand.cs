namespace BerldPokerEngine.Poker
{
    public static class Hand
    {
        public const int Amount = 10;

        public const int HighCard = 0;
        public const int Pair = 1;
        public const int TwoPair = 2;
        public const int ThreeOfAKind = 3;
        public const int Straight = 4;
        public const int Flush = 5;
        public const int FullHouse = 6;
        public const int FourOfAKind = 7;
        public const int StraightFlush = 8;
        public const int RoyalFlush = 9;

        public static string ToFormatString(int hand)
        {
            return hand switch
            {
                HighCard => "High card",
                Pair => "Pair",
                TwoPair => "Two pair",
                ThreeOfAKind => "Three of a kind",
                Straight => "Straight",
                Flush => "Flush",
                FullHouse => "Full house",
                FourOfAKind => "Four of a kind",
                StraightFlush => "Straight flush",
                RoyalFlush => "Royal flush",
                _ => string.Empty
            };
        }

        public static string GetTabPadding(int hand)
        {
            return hand switch
            {
                Pair or Flush => "\t\t\t",
                ThreeOfAKind => "\t",
                _ => "\t\t",
            };
        }
    }
}
