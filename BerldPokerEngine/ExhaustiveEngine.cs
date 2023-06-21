using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    public static class ExhaustiveEngine
    {
        public static List<Player> Evaluate(List<Card>? boardCards, List<List<Card>?> holeCards)
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

            List<int> winners = new();
            Card[] cardsToEvaluate = new Card[Engine.CardsToEvaluateAmount];

            Action<int[]> evaluateAction = (wildCardIndexes) =>
                Engine.DoIteration(wildCardIndexes, validBoardCards, aliveCards, players, winners, cardsToEvaluate);

            int wildCardOffset = 0;

            // Special case with no opponents
            if (players.Count == 1)
            {
                evaluateAction = NestIterateCombinations(wildCardAmount, aliveCards.Count, wildCardOffset, evaluateAction);
            }
            else
            {
                if (wildBoardCardAmount > 0)
                {
                    evaluateAction = NestIterateCombinations(wildBoardCardAmount, aliveCards.Count, wildCardOffset, evaluateAction);
                    wildCardOffset += wildBoardCardAmount;
                }

                foreach (Player player in players)
                {
                    if (player.WildCardAmount > 0)
                    {
                        evaluateAction = NestIterateCombinations(player.WildCardAmount, aliveCards.Count, wildCardOffset, evaluateAction);
                        wildCardOffset += player.WildCardAmount;
                    }
                }
            }

            evaluateAction(new int[wildCardAmount]);
            return players.OrderBy(c => c.Index).ToList();
        }

        public static long CalculateIterationAmount(List<Card>? boardCards, List<List<Card>?> holeCards)
        {
            List<Card> validBoardCards = Engine.EnsureValidBoardCards(boardCards);
            List<List<Card>> validHoleCards = Engine.EnsureValidHoleCards(holeCards);

            List<Player> players = Engine.GetPlayersFromHoleCards(validHoleCards);

            Engine.EnsureNoDuplicateCards(players, validBoardCards);
            Engine.EnsureEnoughCardsAlive(players.Count);

            int wildBoardCardAmount = Engine.GetWildBoardCardAmount(validBoardCards.Count);
            int wildPlayerCardAmount = Engine.GetWildPlayerCardAmount(players);
            int wildCardAmount = wildBoardCardAmount + wildPlayerCardAmount;

            List<Card> aliveCards = Engine.GetAliveCards(players, validBoardCards);

            int cardsLeftAmount = aliveCards.Count;
            long iterationAmount = 1;

            // Special case with no opponents
            if (players.Count == 1)
            {
                iterationAmount *= GetBinCoeff(cardsLeftAmount, wildCardAmount);
            }
            else
            {
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
