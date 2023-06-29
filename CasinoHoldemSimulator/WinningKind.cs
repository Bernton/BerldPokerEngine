namespace CasinoHoldemSimulator
{
    internal static class WinningKind
    {
        internal const int Amount = 8;

        internal const int WinAnteDefault = 0;
        internal const int WinAnteFlush = 1;
        internal const int WinAnteFullHouse = 2;
        internal const int WinAnteFourOfAKind = 3;
        internal const int WinAnteStraightFlush = 4;
        internal const int WinAnteRoyalFlush = 5;
        internal const int WinContinueBet = 6;
        internal const int LossContinue = 7;

        internal static string ToString(int winningKind)
        {
            return winningKind switch
            {
                WinAnteDefault => "Straight or less",
                WinAnteFlush => "Flush",
                WinAnteFullHouse => "Full house",
                WinAnteFourOfAKind => "Four of a kind",
                WinAnteStraightFlush => "Straight flush",
                WinAnteRoyalFlush => "Royal flush",
                WinContinueBet => "Win on continue",
                LossContinue => "Loss on continue",
                _ => string.Empty
            };
        }

        internal static string GetPadding(int winningKind)
        {
            return winningKind switch
            {
                WinAnteFlush => "\t\t\t",
                WinAnteDefault or WinContinueBet or LossContinue => "\t",
                _ => "\t\t"
            };
        }
    }
}
