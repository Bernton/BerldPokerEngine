namespace BerldPokerEngine.Poker
{
    internal static class Suit
    {
        internal const int Clubs = 0;
        internal const int Diamonds = 1;
        internal const int Hearts = 2;
        internal const int Spades = 3;

        internal static char ToChar(int suit)
        {
            return suit switch
            {
                Clubs => 'c',
                Diamonds => 'd',
                Hearts => 'h',
                Spades => 's',
                _ => default
            };
        }

        internal static int? FromChar(char suitChar)
        {
            return suitChar switch
            {
                'c' => Clubs,
                'd' => Diamonds,
                'h' => Hearts,
                's' => Spades,
                _ => null
            };
        }
    }
}
