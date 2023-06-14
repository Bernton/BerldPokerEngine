namespace BerldPokerEngine.Poker
{
    internal class Player
    {
        internal int Index { get; }
        internal int WildCardAmount => 2 - HoleCards.Count;
        internal List<Card> HoleCards { get; }
        internal double[] Equities { get; }
        internal HandValue Value { get; }

        internal Player(int index, List<Card> holeCards)
        {
            Index = index;
            HoleCards = new();
            HoleCards.AddRange(holeCards);
            Equities = new double[Hand.Amount];
            Value = new HandValue();
        }
    }
}
