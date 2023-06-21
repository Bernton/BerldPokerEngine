using BerldPokerEngine.Poker;
using System.Security.Cryptography;

namespace BerldPokerEngine
{
    public class RandomEngine
    {
        public static List<Player> Evaluate(List<Card>? boardCards, List<List<Card>?> holeCards, int iterationAmount)
        {
            List<Card> validBoardCards = Engine.EnsureValidBoardCards(boardCards);
            List<List<Card>> validHoleCards = Engine.EnsureValidHoleCards(holeCards);

            List<Player> players =
                Engine.GetPlayersFromHoleCards(validHoleCards)
                .OrderBy(c => c.HoleCards.Count).ToList();

            Engine.EnsureNoDuplicateCards(players, validBoardCards);
            Engine.EnsureEnoughCardsAlive(players.Count);

            int wildBoardCardAmount = Engine.GetWildBoardCardAmount(validBoardCards.Count);
            int wildPlayerCardAmount = Engine.GetWildPlayerCardAmount(players);
            int wildCardAmount = wildBoardCardAmount + wildPlayerCardAmount;

            List<Card> aliveCards = Engine.GetAliveCards(players, validBoardCards);
            int[] aliveCardIndexes = new int[aliveCards.Count];
            int[] wildCardIndexes = new int[wildCardAmount];

            List<int> winners = new();
            Card[] cardsToEvaluate = new Card[Engine.CardsToEvaluateAmount];

            for (int iterationI = 0; iterationI < iterationAmount; iterationI++)
            {
                for (int i = 0; i < aliveCardIndexes.Length; i++)
                {
                    aliveCardIndexes[i] = i;
                }

                for (int i = 0; i < wildCardIndexes.Length; i++)
                {
                    int chosenIndex = RandomNumberGenerator.GetInt32(aliveCardIndexes.Length - i);
                    wildCardIndexes[i] = aliveCardIndexes[chosenIndex];
                    aliveCardIndexes[chosenIndex] = aliveCardIndexes[^(1 + i)];
                }

                Engine.DoIteration(wildCardIndexes, validBoardCards, aliveCards, players, winners, cardsToEvaluate);
            }

            return players.OrderBy(c => c.Index).ToList();
        }
    }
}
