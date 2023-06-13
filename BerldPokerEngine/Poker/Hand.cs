namespace BerldPokerEngine.Poker
{
    internal static class Hand
    {
        internal const int HighCard = 0;
        internal const int Pair = 1;
        internal const int TwoPair = 2;
        internal const int ThreeOfAKind = 3;
        internal const int Straight = 4;
        internal const int Flush = 5;
        internal const int FullHouse = 6;
        internal const int FourOfAKind = 7;
        internal const int StraightFlush = 8;
        internal const int RoyalFlush = 9;

        internal static string ToFormatString(int hand)
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

        internal static string GetTabPadding(int hand)
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
