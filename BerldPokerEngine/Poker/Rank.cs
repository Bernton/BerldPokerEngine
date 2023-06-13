namespace BerldPokerEngine.Poker
{
    internal static class Rank
    {
        internal const int Deuce = 0;
        internal const int Tray = 1;
        internal const int Four = 2;
        internal const int Five = 3;
        internal const int Six = 4;
        internal const int Seven = 5;
        internal const int Eight = 6;
        internal const int Nine = 7;
        internal const int Ten = 8;
        internal const int Jack = 9;
        internal const int Queen = 10;
        internal const int King = 11;
        internal const int Ace = 12;

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

        internal static int? FromChar(char rankChar)
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
