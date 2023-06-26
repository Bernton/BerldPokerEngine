using BerldPokerEngine.Poker;
using System.Security.Cryptography;

namespace BerldPokerEngine
{
    public class RandomEngine
    {
        public static List<Player> Evaluate(List<Card>? boardCards, List<List<Card>?> holeCards, int iterationAmount)
        {
            EngineData data = new(boardCards, holeCards);

            int[] aliveCardIndexes = new int[data.AliveCards.Count];

            for (int iterationI = 0; iterationI < iterationAmount; iterationI++)
            {
                for (int i = 0; i < aliveCardIndexes.Length; i++)
                {
                    aliveCardIndexes[i] = i;
                }

                for (int i = 0; i < data.WildCardIndexes.Length; i++)
                {
                    int chosenIndex = RandomNumberGenerator.GetInt32(aliveCardIndexes.Length - i);
                    data.WildCardIndexes[i] = aliveCardIndexes[chosenIndex];
                    aliveCardIndexes[chosenIndex] = aliveCardIndexes[^(1 + i)];
                }

                Engine.DoIteration(data.WildCardIndexes, data.BoardCards, data.AliveCards, data.Players, data.Winners, data.CardsToEvaluate);
            }

            return data.Players.OrderBy(c => c.Index).ToList();
        }
    }
}
