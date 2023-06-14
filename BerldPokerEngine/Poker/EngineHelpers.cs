namespace BerldPokerEngine.Poker
{
    internal static class EngineHelpers
    {
        private const int AllCardsAmount = 52;

        internal static List<Player> GetPlayersFromHoleCards(List<List<Card>> holeCards)
        {
            return holeCards.Select((holeCards, index) => new Player(index, holeCards)).ToList();
        }

        internal static List<Card> GetAliveCards(List<Player> players)
        {
            List<Card> seed = new();
            List<Card> deadCards = players.Aggregate(seed, (a, b) => Enumerable.Concat(a, b.HoleCards).ToList());
            return GetAllCards().Except(deadCards).ToList();
        }

        private static List<Card> GetAllCards()
        {
            List<Card> cards = new();

            for (int i = 0; i < AllCardsAmount; i++)
            {
                cards.Add(new Card(i));
            }

            return cards;
        }
    }
}
