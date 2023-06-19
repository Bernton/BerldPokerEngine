namespace BerldPokerEngine.Poker
{
    public static class Suit
    {
        internal const int Amount = 4;

        public const int Clubs = 0;
        public const int Diamonds = 1;
        public const int Hearts = 2;
        public const int Spades = 3;

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

        public static int? FromChar(char suitChar)
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
