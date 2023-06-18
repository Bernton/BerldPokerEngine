namespace BerldPokerEngine.Poker
{
    internal static class EngineHelpers
    {
        internal const int CardsToEvaluateAmount = 7;
        internal const int BoardCardAmount = 5;
        private const int PlayerCardAmount = 2;
        private const int AllCardsAmount = 52;

        internal static int GetWildPlayerCardAmount(List<Player> players)
        {
            return players.Count * PlayerCardAmount - players.Sum(c => c.HoleCards.Count);
        }

        internal static List<Player> GetPlayersFromHoleCards(List<List<Card>> holeCards)
        {
            return holeCards.Select((holeCards, index) => new Player(index, holeCards)).ToList();
        }

        internal static List<Card> GetAliveCards(List<Player> players, List<Card> boardCards)
        {
            List<Card> seed = new(boardCards);
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
