using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    public class RandomEngine
    {
        public static List<Player> Evaluate(List<Card>? boardCards, List<List<Card>?> holeCards, int iterationAmount)
        {
            Random random = new();
            EngineData data = new(boardCards, holeCards);

            int[] aliveCardIndexes = new int[data.AliveCards.Count];
            int[] wildCardIndexes = new int[data.WildCardAmount];

            for (int iterationI = 0; iterationI < iterationAmount; iterationI++)
            {
                for (int i = 0; i < aliveCardIndexes.Length; i++)
                {
                    aliveCardIndexes[i] = i;
                }

                for (int i = 0; i < wildCardIndexes.Length; i++)
                {
                    int chosenIndex = random.Next(aliveCardIndexes.Length - i);
                    wildCardIndexes[i] = aliveCardIndexes[chosenIndex];
                    aliveCardIndexes[chosenIndex] = aliveCardIndexes[^(1 + i)];
                }

                Engine.DoIteration(wildCardIndexes, data);
            }

            return data.Players.OrderBy(c => c.Index).ToList();
        }
    }
}
