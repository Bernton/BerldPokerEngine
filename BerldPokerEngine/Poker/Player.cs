namespace BerldPokerEngine.Poker
{
    internal class Player
    {
        internal int Index { get; }
        internal List<Card> HoleCards { get; }
        internal double[] Equities { get; }
        internal HandValue Value { get; }
        internal int WildCardAmount => 2 - HoleCards.Count;

        internal Player(int index, List<Card> holeCards)
        {
            Index = index;
            HoleCards = new(holeCards);
            Equities = new double[Hand.Amount];
            Value = new HandValue();
        }
    }
}
