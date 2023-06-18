﻿using static BerldPokerEngine.Poker.EngineHelpers;

namespace BerldPokerEngine.Poker
{
    internal static class Engine
    {
        internal static List<Player>? Evaluate(List<Card> boardCards, List<List<Card>> holeCards)
        {
            List<Player> players = GetPlayersFromHoleCards(holeCards);
            players = players.OrderBy(c => c.HoleCards.Count).ToList();

            int wildBoardCardAmount = BoardCardAmount - boardCards.Count;
            int wildPlayerCardAmount = GetWildPlayerCardAmount(players);
            int wildCardAmount = wildBoardCardAmount + wildPlayerCardAmount;

            List<Card> aliveCards = GetAliveCards(players, boardCards);

            List<int> winners = new();
            Card[] cardsToEvaluate = new Card[CardsToEvaluateAmount];

            Action<int[]> evaluateAction = (int[] wildCardIndexes) =>
                DoIteration(wildCardIndexes, boardCards, aliveCards, ref players, ref winners, ref cardsToEvaluate);

            int wildCardOffset = 0;

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

            evaluateAction(new int[wildCardAmount]);
            return players;
        }

        private static Action<int[]> NestIterateCombinations(int wildCardAmount, int loopBound, int wildCardOffset, Action<int[]> iterationAction)
        {
            return (int[] wildCardIndexes) =>
                IterateCombinations(wildCardIndexes, wildCardOffset, wildCardAmount, loopBound, iterationAction);
        }

        private static void DoIteration(
            int[] wildCardIndexes,
            List<Card> boardCards,
            List<Card> aliveCards,
            ref List<Player> players,
            ref List<int> winners,
            ref Card[] cardsToEvaluate)
        {
            int cardsToEvaluateI = 0;
            int wildCardI = 0;

            for (int boardCardI = 0; boardCardI < 5; boardCardI++)
            {
                Card boardCardToEvaluate;

                if (boardCardI < boardCards.Count)
                {
                    boardCardToEvaluate = boardCards[boardCardI];
                }
                else
                {
                    boardCardToEvaluate = aliveCards[wildCardIndexes[wildCardI++]];
                }

                cardsToEvaluate[cardsToEvaluateI++] = boardCardToEvaluate;
            }

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];

                for (int playerCardI = 0; playerCardI < 2; playerCardI++)
                {
                    Card playerCardToEvaluate;

                    if (playerCardI < player.HoleCards.Count)
                    {
                        playerCardToEvaluate = player.HoleCards[playerCardI];
                    }
                    else
                    {
                        playerCardToEvaluate = aliveCards[wildCardIndexes[wildCardI++]];
                    }

                    cardsToEvaluate[cardsToEvaluateI + playerCardI] = playerCardToEvaluate;
                }

                EvaluateCards(cardsToEvaluate, ref player);
            }

            AddEquityToWinners(ref players, ref winners);
        }

        private static void IterateCombinations(int[] indexes, int startIndex, int loopAmount, int loopBound, Action<int[]> action)
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

        private static void AddEquityToWinners(ref List<Player> players, ref List<int> winners)
        {
            winners.Clear();
            winners.Add(0);

            for (int i = 1; i < players.Count; i++)
            {
                HandValue value = players[i].Value;
                HandValue winnerValue = players[winners[0]].Value;

                int comparison = value.Hand - winnerValue.Hand;

                if (comparison == 0)
                {
                    for (int ranksI = value.Ranks.Length - 1; ranksI >= 0; ranksI--)
                    {
                        if (value.Ranks[ranksI] < 0)
                        {
                            break;
                        }

                        comparison = value.Ranks[ranksI] - winnerValue.Ranks[ranksI];

                        if (comparison != 0)
                        {
                            break;
                        }
                    }
                }

                if (comparison > 0)
                {
                    winners.Clear();
                    winners.Add(i);
                }
                else if (comparison == 0)
                {
                    winners.Add(i);
                }
            }

            double winnerEquity = 1.0 / winners.Count;

            for (int i = 0; i < winners.Count; i++)
            {
                Player winner = players[winners[i]];
                int handIndex = winner.Value.Hand;
                winner.Equities[handIndex] += winnerEquity;
            }
        }

        private static readonly bool[] _coveredFlushRanks = new bool[Rank.Amount];

        private static void SetCoveredFlushRanks(Card[] cards, int flushSuit)
        {
            for (int i = 0; i < Rank.Amount; i++)
            {
                _coveredFlushRanks[i] = false;
            }

            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];

                if (card.Suit == flushSuit)
                {
                    _coveredFlushRanks[card.Rank] = true;
                }
            }
        }

        private static void EvaluateCards(Card[] cards, ref Player player)
        {
            // Straight flush
            int[] suitAmounts = new int[Suit.Amount];
            int? flushSuit = null;

            for (int i = 0; i < cards.Length; i++)
            {
                int cardSuit = cards[i].Suit;
                suitAmounts[cardSuit]++;

                if (suitAmounts[cardSuit] == 5)
                {
                    flushSuit = cardSuit;
                }
            }

            if (flushSuit.HasValue)
            {
                SetCoveredFlushRanks(cards, flushSuit.Value);

                int consecutiveFlushAmount = 0;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (!_coveredFlushRanks[i])
                    {
                        consecutiveFlushAmount = 0;
                        continue;
                    }

                    consecutiveFlushAmount++;

                    if (consecutiveFlushAmount == 5)
                    {
                        player.Value.Hand = i == Rank.Ten ? Hand.RoyalFlush : Hand.StraightFlush;
                        player.Value.Ranks[4] = i + 4;
                        player.Value.Ranks[3] = -1;
                        return;
                    }
                    else if (consecutiveFlushAmount == 4 && i == Rank.Deuce && _coveredFlushRanks[Rank.Ace])
                    {
                        player.Value.Hand = Hand.StraightFlush;
                        player.Value.Ranks[4] = Rank.Five;
                        player.Value.Ranks[3] = -1;
                        return;
                    }
                }
            }

            int[] rankAmounts = new int[Rank.Amount];

            for (int i = 0; i < cards.Length; i++)
            {
                rankAmounts[cards[i].Rank]++;
            }

            // Four of a kind
            for (int i = Rank.Ace; i >= Rank.Deuce; i--)
            {
                if (rankAmounts[i] == 4)
                {
                    for (int j = Rank.Ace; j >= Rank.Deuce; j--)
                    {
                        if (rankAmounts[j] > 0 && j != i)
                        {
                            player.Value.Hand = Hand.FourOfAKind;
                            player.Value.Ranks[4] = i;
                            player.Value.Ranks[3] = j;
                            player.Value.Ranks[2] = -1;
                            return;
                        }
                    }
                }
            }

            // Full house
            int? threeOfAKindRank = null;

            for (int i = Rank.Ace; i >= Rank.Deuce; i--)
            {
                if (rankAmounts[i] == 3)
                {
                    threeOfAKindRank = i;
                    break;
                }
            }

            if (threeOfAKindRank.HasValue)
            {
                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (rankAmounts[i] >= 2 && i != threeOfAKindRank.Value)
                    {
                        player.Value.Hand = Hand.FullHouse;
                        player.Value.Ranks[4] = threeOfAKindRank.Value;
                        player.Value.Ranks[3] = i;
                        player.Value.Ranks[2] = -1;
                        return;
                    }
                }
            }

            // Flush
            if (flushSuit.HasValue)
            {
                player.Value.Hand = Hand.Flush;
                int ranksIndex = 4;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (_coveredFlushRanks[i])
                    {
                        player.Value.Ranks[ranksIndex] = i;
                        ranksIndex--;

                        if (ranksIndex < 0)
                        {
                            break;
                        }
                    }
                }

                return;
            }

            // Straight
            int consecutiveAmount = 0;

            for (int i = Rank.Ace; i >= Rank.Deuce; i--)
            {
                if (rankAmounts[i] == 0)
                {
                    consecutiveAmount = 0;
                    continue;
                }

                consecutiveAmount++;

                if (consecutiveAmount == 5)
                {
                    player.Value.Hand = Hand.Straight;
                    player.Value.Ranks[4] = i + 4;
                    player.Value.Ranks[3] = -1;
                    return;
                }
                else if (consecutiveAmount == 4 && i == Rank.Deuce && rankAmounts[Rank.Ace] > 0)
                {
                    player.Value.Hand = Hand.Straight;
                    player.Value.Ranks[4] = Rank.Five;
                    player.Value.Ranks[3] = -1;
                    return;
                }
            }

            // Three of a kind
            if (threeOfAKindRank.HasValue)
            {
                player.Value.Hand = Hand.ThreeOfAKind;
                player.Value.Ranks[4] = threeOfAKindRank.Value;
                player.Value.Ranks[1] = -1;

                int ranksIndex = 3;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (rankAmounts[i] > 0 && i != threeOfAKindRank.Value)
                    {
                        player.Value.Ranks[ranksIndex] = i;
                        ranksIndex--;

                        if (ranksIndex < 2)
                        {
                            break;
                        }
                    }
                }

                return;
            }

            // Two pair
            int? highestPairRank = null;

            for (int i = Rank.Ace; i >= Rank.Deuce; i--)
            {
                if (rankAmounts[i] == 2)
                {
                    highestPairRank = i;
                    break;
                }
            }

            if (highestPairRank.HasValue)
            {
                int? secondPairRank = null;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (rankAmounts[i] == 2 && i != highestPairRank.Value)
                    {
                        secondPairRank = i;
                        break;
                    }
                }

                if (secondPairRank.HasValue)
                {
                    for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                    {
                        if (rankAmounts[i] > 0 && i != highestPairRank.Value && i != secondPairRank.Value)
                        {
                            player.Value.Hand = Hand.TwoPair;
                            player.Value.Ranks[4] = highestPairRank.Value;
                            player.Value.Ranks[3] = secondPairRank.Value;
                            player.Value.Ranks[2] = i;
                            player.Value.Ranks[1] = -1;
                            return;
                        }
                    }
                }
            }

            // Pair
            if (highestPairRank.HasValue)
            {
                player.Value.Hand = Hand.Pair;
                player.Value.Ranks[4] = highestPairRank.Value;
                player.Value.Ranks[0] = -1;

                int ranksIndex = 3;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (rankAmounts[i] > 0 && i != highestPairRank.Value)
                    {
                        player.Value.Ranks[ranksIndex] = i;
                        ranksIndex--;

                        if (ranksIndex < 1)
                        {
                            break;
                        }
                    }
                }

                return;
            }

            // High card
            player.Value.Hand = Hand.HighCard;
            int highCardRanksIndex = 4;

            for (int i = Rank.Ace; i >= Rank.Deuce; i--)
            {
                if (rankAmounts[i] > 0)
                {
                    player.Value.Ranks[highCardRanksIndex] = i;
                    highCardRanksIndex--;

                    if (highCardRanksIndex < 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}