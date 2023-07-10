using BerldPokerEngine.Poker;

namespace UltimateTexasHoldemSimulator
{
    internal class DistinctFlopHolding : DistinctHolding
    {
        public bool IsEvaluated { get; set; } = false;

        public double CheckWinnings { get; set; } = 0;
        public double Raise3Winnings { get; set; } = 0;
        public double Raise4Winnings { get; set; } = 0;

        internal DistinctFlopHolding(Card[] cards, List<int>? sortMarkers) : base(cards, sortMarkers)
        {
        }
    }
}
