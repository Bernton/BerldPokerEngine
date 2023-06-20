using System.Collections.ObjectModel;

namespace BerldPokerEngine.Poker
{
    public class Player
    {
        public int Index { get; }

        public double TotalEquity => Equities.Sum();
        public long TotalNegativeEquity => NegativeEquities.Sum();

        public long TotalWinEquity => WinEquities.Sum();
        public double TotalTieEquity => TieEquities.Sum();

        public ReadOnlyCollection<double> EquityAmounts => new(Equities);
        public ReadOnlyCollection<long> NegativeEquityAmounts => new(NegativeEquities);

        public ReadOnlyCollection<long> WinEquityAmounts => new(WinEquities);
        public ReadOnlyCollection<double> TieEquityAmounts => new(TieEquities);

        internal List<Card> HoleCards { get; }
        internal double[] Equities { get; }
        internal long[] NegativeEquities { get; }
        internal long[] WinEquities { get; }
        internal double[] TieEquities { get; }
        internal HandValue Value { get; }
        internal int WildCardAmount => 2 - HoleCards.Count;

        internal Player(int index, List<Card> holeCards)
        {
            Index = index;
            HoleCards = new(holeCards);
            Equities = new double[Hand.Amount];
            NegativeEquities = new long[Hand.Amount];
            WinEquities = new long[Hand.Amount];
            TieEquities = new double[Hand.Amount];
            Value = new HandValue();
        }
    }
}
