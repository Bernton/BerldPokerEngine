using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    internal class EngineData
    {
        internal const int BoardCardAmount = 5;
        internal const int PlayerCardAmount = 2;
        private const int AllCardsAmount = 52;
        private const int CardsToEvaluateAmount = 7;

        internal List<Card> BoardCards { get; }
        internal List<Player> Players { get; }
        internal int WildBoardCardAmount { get; }
        internal int WildPlayerCardAmount { get; }
        internal int WildCardAmount { get; }
        internal List<Card> AliveCards { get; }
        internal List<int> Winners { get; }
        internal Card[] CardsToEvaluate { get; }
        internal int[] WildCardIndexes { get; }

        internal EngineData(List<Card>? boardCards, List<List<Card>?> holeCards)
        {
            BoardCards = EnsureValidBoardCards(boardCards);
            List<List<Card>> validHoleCards = EnsureValidHoleCards(holeCards);

            Players =
                GetPlayersFromHoleCards(validHoleCards)
                .OrderBy(c => c.HoleCards.Count).ToList();

            EnsureNoDuplicateCards(Players, BoardCards);
            EnsureEnoughCardsAlive(Players.Count);

            WildBoardCardAmount = GetWildBoardCardAmount(BoardCards.Count);
            WildPlayerCardAmount = GetWildPlayerCardAmount(Players);
            WildCardAmount = WildBoardCardAmount + WildPlayerCardAmount;

            AliveCards = GetAliveCards(Players, BoardCards);

            Winners = new();
            CardsToEvaluate = new Card[CardsToEvaluateAmount];
            WildCardIndexes = new int[WildCardAmount];
        }

        private static List<Card> EnsureValidBoardCards(List<Card>? boardCards)
        {
            if (boardCards is null)
            {
                boardCards ??= new();
            }
            else if (boardCards.Count > BoardCardAmount)
            {
                throw new ArgumentException($"{nameof(boardCards)} must have {BoardCardAmount} or fewer cards.");
            }

            return boardCards;
        }

        private static List<List<Card>> EnsureValidHoleCards(List<List<Card>?> holeCards)
        {
            List<List<Card>> validHoleCards = new();

            if (holeCards.Count == 0)
            {
                throw new ArgumentException($"{nameof(holeCards)} must not be empty.");
            }

            foreach (List<Card>? playerCards in holeCards)
            {
                if (playerCards is not null && playerCards.Count > PlayerCardAmount)
                {
                    throw new ArgumentException($"{nameof(playerCards)} must have {PlayerCardAmount} or fewer cards.");
                }

                List<Card> validPlayerCards = playerCards ?? new();
                validHoleCards.Add(validPlayerCards);
            }

            return validHoleCards;
        }

        private static List<Player> GetPlayersFromHoleCards(List<List<Card>> holeCards)
        {
            return holeCards.Select((holeCards, index) => new Player(index, holeCards)).ToList();
        }

        private static void EnsureNoDuplicateCards(List<Player> players, List<Card> boardCards)
        {
            List<Card> deadCards = GetDeadCards(players, boardCards);

            if (deadCards.Distinct().Count() != deadCards.Count)
            {
                throw new ArgumentException("Duplicate cards must not exist.");
            }
        }

        private static void EnsureEnoughCardsAlive(int playerAmount)
        {
            int deadCardAmount = BoardCardAmount + playerAmount * PlayerCardAmount;

            if (deadCardAmount > AllCardsAmount)
            {
                throw new ArgumentException("There must be enough alive cards.");
            }
        }

        private static int GetWildBoardCardAmount(int boardCards)
        {
            return BoardCardAmount - boardCards;
        }

        private static int GetWildPlayerCardAmount(List<Player> players)
        {
            return players.Count * PlayerCardAmount - players.Sum(c => c.HoleCards.Count);
        }

        private static List<Card> GetAliveCards(List<Player> players, List<Card> boardCards)
        {
            List<Card> deadCards = GetDeadCards(players, boardCards);
            return GetAllCards().Except(deadCards).ToList();
        }

        private static List<Card> GetDeadCards(List<Player> players, List<Card> boardCards)
        {
            List<Card> seed = new(boardCards);
            List<Card> deadCards = players.Aggregate(seed, (a, b) => a.Concat(b.HoleCards).ToList());
            return deadCards;
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
