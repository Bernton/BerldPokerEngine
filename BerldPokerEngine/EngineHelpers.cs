using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    internal static class EngineHelpers
    {
        internal const int CardsToEvaluateAmount = 7;
        internal const int BoardCardAmount = 5;
        private const int PlayerCardAmount = 2;
        private const int AllCardsAmount = 52;

        internal static List<Card> EnsureValidBoardCards(List<Card>? boardCards)
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

        internal static List<List<Card>> EnsureValidHoleCards(List<List<Card>?> holeCards)
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

        internal static void EnsureNoDuplicateCards(List<Player> players, List<Card> boardCards)
        {
            List<Card> deadCards = GetDeadCards(players, boardCards);

            if (deadCards.Distinct().Count() != deadCards.Count)
            {
                throw new ArgumentException("Duplicate cards must not exist.");
            }
        }

        internal static void EnsureEnoughCardsAlive(int playerAmount)
        {
            int deadCardAmount = BoardCardAmount + playerAmount * PlayerCardAmount;

            if (deadCardAmount > AllCardsAmount)
            {
                throw new ArgumentException("There must be enough alive cards.");
            }
        }

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
            List<Card> deadCards = GetDeadCards(players, boardCards);
            return GetAllCards().Except(deadCards).ToList();
        }

        internal static long CalculateIterationAmount(List<Card>? boardCards, List<List<Card>?> holeCards)
        {
            List<Card> validBoardCards = EnsureValidBoardCards(boardCards);
            List<List<Card>> validHoleCards = EnsureValidHoleCards(holeCards);
            List<Player> players = GetPlayersFromHoleCards(validHoleCards);
            List<Card> aliveCards = GetAliveCards(players, validBoardCards);

            int cardsLeftAmount = aliveCards.Count;
            long iterationAmount = 1;

            int wildBoardCardAmount = BoardCardAmount - validBoardCards.Count;

            if (wildBoardCardAmount > 0)
            {
                iterationAmount *= GetBinCoeff(cardsLeftAmount, wildBoardCardAmount);
                cardsLeftAmount -= wildBoardCardAmount;
            }

            foreach (Player player in players)
            {
                if (player.WildCardAmount > 0)
                {
                    iterationAmount *= GetBinCoeff(cardsLeftAmount, player.WildCardAmount);
                    cardsLeftAmount -= player.WildCardAmount;
                }
            }

            return iterationAmount;
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

        private static long GetBinCoeff(long N, long K)
        {
            // This function gets the total number of unique combinations based upon N and K.
            // N is the total number of items.
            // K is the size of the group.
            // Total number of unique combinations = N! / ( K! (N - K)! ).
            // This function is less efficient, but is more likely to not overflow when N and K are large.
            // Taken from:  http://blog.plover.com/math/choose.html
            //
            long r = 1;
            long d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }
    }
}
