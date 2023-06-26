using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    public static class ExhaustiveEngine
    {
        public static List<Player> Evaluate(List<Card>? boardCards, List<List<Card>?> holeCards)
        {
            EngineData data = new(boardCards, holeCards);

            Action<int[]> evaluateAction = (wildCardIndexes) =>
                Engine.DoIteration(wildCardIndexes, data.BoardCards, data.AliveCards, data.Players, data.Winners, data.CardsToEvaluate);

            int wildCardOffset = 0;

            // Special case with no opponents
            if (data.Players.Count == 1)
            {
                evaluateAction = NestIterateCombinations(data.WildCardAmount, data.AliveCards.Count, wildCardOffset, evaluateAction);
            }
            else
            {
                if (data.WildBoardCardAmount > 0)
                {
                    evaluateAction = NestIterateCombinations(data.WildBoardCardAmount, data.AliveCards.Count, wildCardOffset, evaluateAction);
                    wildCardOffset += data.WildBoardCardAmount;
                }

                foreach (Player player in data.Players)
                {
                    if (player.WildCardAmount > 0)
                    {
                        evaluateAction = NestIterateCombinations(player.WildCardAmount, data.AliveCards.Count, wildCardOffset, evaluateAction);
                        wildCardOffset += player.WildCardAmount;
                    }
                }
            }

            evaluateAction(data.WildCardIndexes);
            return data.Players.OrderBy(c => c.Index).ToList();
        }

        public static long CalculateIterationAmount(List<Card>? boardCards, List<List<Card>?> holeCards)
        {
            EngineData data = new(boardCards, holeCards);

            int cardsLeftAmount = data.AliveCards.Count;
            long iterationAmount = 1;

            // Special case with no opponents
            if (data.Players.Count == 1)
            {
                iterationAmount *= GetBinCoeff(cardsLeftAmount, data.WildCardAmount);
            }
            else
            {
                if (data.WildBoardCardAmount > 0)
                {
                    iterationAmount *= GetBinCoeff(cardsLeftAmount, data.WildBoardCardAmount);
                    cardsLeftAmount -= data.WildBoardCardAmount;
                }

                foreach (Player player in data.Players)
                {
                    if (player.WildCardAmount > 0)
                    {
                        iterationAmount *= GetBinCoeff(cardsLeftAmount, player.WildCardAmount);
                        cardsLeftAmount -= player.WildCardAmount;
                    }
                }
            }

            return iterationAmount;
        }

        private static Action<int[]> NestIterateCombinations(int wildCardAmount, int loopBound, int wildCardOffset,
            Action<int[]> iterationAction)
        {
            return (wildCardIndexes) =>
                IterateCombinations(wildCardIndexes, wildCardOffset, wildCardAmount, loopBound, iterationAction);
        }

        private static void IterateCombinations(int[] indexes, int startIndex, int loopAmount, int loopBound,
            Action<int[]> action)
        {
            int[] counters = new int[loopAmount];

            for (int u = 0; u < loopAmount; u++)
            {
                counters[u] = u;
            }

            bool breakOuter = false;
            int last = loopAmount - 1;

            while (!breakOuter)
            {
                // Apply 'counters' to 'indexes' and check for duplicates
                bool hasDuplicate = false;

                for (int i = 0; i < loopAmount; i++)
                {
                    for (int upperI = indexes.Length - 1; upperI >= startIndex + loopAmount; upperI--)
                    {
                        if (counters[i] == indexes[upperI])
                        {
                            hasDuplicate = true;
                            break;
                        }
                    }

                    if (hasDuplicate) break;

                    indexes[i + startIndex] = counters[i];
                }

                if (!hasDuplicate)
                {
                    action(indexes);
                }

                if (last < 0) break;

                counters[last]++;

                for (int i = 0; i < loopAmount; i++)
                {
                    int iLast = loopAmount - i - 1;
                    int bound = loopBound - i;

                    if (counters[iLast] < bound) break;

                    if (i == last)
                    {
                        breakOuter = true;
                        break;
                    }

                    int iSecondLast = iLast - 1;
                    counters[iSecondLast]++;

                    if (counters[iSecondLast] < bound - 1)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            counters[iSecondLast + 1 + j] = counters[iSecondLast + j] + 1;
                        }

                        break;
                    }
                }
            }
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
