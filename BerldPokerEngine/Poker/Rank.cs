namespace BerldPokerEngine.Poker
{
    public static class Rank
    {
        internal const int Amount = 13;

        public const int Deuce = 0;
        public const int Tray = 1;
        public const int Four = 2;
        public const int Five = 3;
        public const int Six = 4;
        public const int Seven = 5;
        public const int Eight = 6;
        public const int Nine = 7;
        public const int Ten = 8;
        public const int Jack = 9;
        public const int Queen = 10;
        public const int King = 11;
        public const int Ace = 12;

        internal static char ToChar(int rank)
        {
            return rank switch
            {
                Deuce => '2',
                Tray => '3',
                Four => '4',
                Five => '5',
                Six => '6',
                Seven => '7',
                Eight => '8',
                Nine => '9',
                Ten => 'T',
                Jack => 'J',
                Queen => 'Q',
                King => 'K',
                Ace => 'A',
                _ => default
            };
        }

        public static int? FromChar(char rankChar)
        {
            return rankChar switch
            {
                '2' => Deuce,
                '3' => Tray,
                '4' => Four,
                '5' => Five,
                '6' => Six,
                '7' => Seven,
                '8' => Eight,
                '9' => Nine,
                'T' => Ten,
                'J' => Jack,
                'Q' => Queen,
                'K' => King,
                'A' => Ace,
                _ => null
            };
        }
    }
}
