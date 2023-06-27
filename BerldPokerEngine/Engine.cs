using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    public static class Engine
    {
        internal static void DoIteration(int[] wildCardIndexes, EngineData data)
        {
            int wildCardI = 0;

            for (int boardCardI = 0; boardCardI < EngineData.BoardCardAmount; boardCardI++)
            {
                bool isWildCard = boardCardI >= data.BoardCards.Count;

                Card boardCardToEvaluate = isWildCard ?
                    data.AliveCards[wildCardIndexes[wildCardI++]] :
                    data.BoardCards[boardCardI];

                data.CardsToEvaluate[boardCardI] = boardCardToEvaluate;
            }

            for (int i = 0; i < data.Players.Count; i++)
            {
                Player player = data.Players[i];

                for (int playerCardI = 0; playerCardI < EngineData.PlayerCardAmount; playerCardI++)
                {
                    bool isWildCard = playerCardI >= player.HoleCards.Count;

                    Card playerCardToEvaluate = isWildCard ?
                        data.AliveCards[wildCardIndexes[wildCardI++]] :
                        player.HoleCards[playerCardI];

                    data.CardsToEvaluate[EngineData.BoardCardAmount + playerCardI] = playerCardToEvaluate;
                }

                SetHandValue(data.CardsToEvaluate, player.Value);
            }

            SetWinners(data.Players, data.Winners);
            AddEquities(data.Players, data.Winners);
        }

        public static void SetHandValue(Card[] cards, HandValue value)
        {
            // Straight flush
            Span<int> suitAmounts = stackalloc int[Suit.Amount];
            int? flushSuit = null;

            for (int i = 0; i < cards.Length; i++)
            {
                int cardSuit = cards[i].Suit;
                suitAmounts[cardSuit]++;

                if (suitAmounts[cardSuit] == 5)
                {
                    flushSuit = cardSuit;
                    break;
                }
            }

            Span<bool> coveredFlushRanks = stackalloc bool[Rank.Amount];

            if (flushSuit.HasValue)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    Card card = cards[i];

                    if (card.Suit == flushSuit)
                    {
                        coveredFlushRanks[card.Rank] = true;
                    }
                }

                int consecutiveFlushAmount = 0;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (!coveredFlushRanks[i])
                    {
                        consecutiveFlushAmount = 0;
                        continue;
                    }

                    consecutiveFlushAmount++;

                    if (consecutiveFlushAmount == 5)
                    {
                        value.Hand = i == Rank.Ten ? Hand.RoyalFlush : Hand.StraightFlush;
                        value.Ranks[4] = i + 4;
                        value.Ranks[3] = -1;
                        return;
                    }
                    else if (consecutiveFlushAmount == 4 && i == Rank.Deuce && coveredFlushRanks[Rank.Ace])
                    {
                        value.Hand = Hand.StraightFlush;
                        value.Ranks[4] = Rank.Five;
                        value.Ranks[3] = -1;
                        return;
                    }
                }
            }

            Span<int> rankAmounts = stackalloc int[Rank.Amount];

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
                            value.Hand = Hand.FourOfAKind;
                            value.Ranks[4] = i;
                            value.Ranks[3] = j;
                            value.Ranks[2] = -1;
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
                        value.Hand = Hand.FullHouse;
                        value.Ranks[4] = threeOfAKindRank.Value;
                        value.Ranks[3] = i;
                        value.Ranks[2] = -1;
                        return;
                    }
                }
            }

            // Flush
            if (flushSuit.HasValue)
            {
                value.Hand = Hand.Flush;
                int ranksIndex = 4;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (coveredFlushRanks[i])
                    {
                        value.Ranks[ranksIndex] = i;
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
                    value.Hand = Hand.Straight;
                    value.Ranks[4] = i + 4;
                    value.Ranks[3] = -1;
                    return;
                }
                else if (consecutiveAmount == 4 && i == Rank.Deuce && rankAmounts[Rank.Ace] > 0)
                {
                    value.Hand = Hand.Straight;
                    value.Ranks[4] = Rank.Five;
                    value.Ranks[3] = -1;
                    return;
                }
            }

            // Three of a kind
            if (threeOfAKindRank.HasValue)
            {
                value.Hand = Hand.ThreeOfAKind;
                value.Ranks[4] = threeOfAKindRank.Value;
                value.Ranks[1] = -1;

                int ranksIndex = 3;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (rankAmounts[i] > 0 && i != threeOfAKindRank.Value)
                    {
                        value.Ranks[ranksIndex] = i;
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
                            value.Hand = Hand.TwoPair;
                            value.Ranks[4] = highestPairRank.Value;
                            value.Ranks[3] = secondPairRank.Value;
                            value.Ranks[2] = i;
                            value.Ranks[1] = -1;
                            return;
                        }
                    }
                }
            }

            // Pair
            if (highestPairRank.HasValue)
            {
                value.Hand = Hand.Pair;
                value.Ranks[4] = highestPairRank.Value;
                value.Ranks[0] = -1;

                int ranksIndex = 3;

                for (int i = Rank.Ace; i >= Rank.Deuce; i--)
                {
                    if (rankAmounts[i] > 0 && i != highestPairRank.Value)
                    {
                        value.Ranks[ranksIndex] = i;
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
            value.Hand = Hand.HighCard;
            int highCardRanksIndex = 4;

            for (int i = Rank.Ace; i >= Rank.Deuce; i--)
            {
                if (rankAmounts[i] > 0)
                {
                    value.Ranks[highCardRanksIndex] = i;
                    highCardRanksIndex--;

                    if (highCardRanksIndex < 0)
                    {
                        break;
                    }
                }
            }
        }

        private static void SetWinners(List<Player> players, List<int> winners)
        {
            winners.Clear();
            winners.Add(0);

            for (int i = 1; i < players.Count; i++)
            {
                HandValue value = players[i].Value;
                HandValue winnerValue = players[winners[0]].Value;

                int comparison = value.CompareTo(winnerValue);

                if (comparison > 0)
                {
                    winners.Clear();
                }

                if (comparison >= 0)
                {
                    winners.Add(i);
                }
            }
        }

        private static void AddEquities(List<Player> players, List<int> winners)
        {
            // Win
            if (winners.Count == 1)
            {
                Player winner = players[winners[0]];
                int handIndex = winner.Value.Hand;
                winner.Equities[handIndex] += 1.0;
                winner.WinEquities[handIndex] += 1;
            }
            else
            {
                // Tie
                double tieEquity = 1.0 / winners.Count;

                for (int i = 0; i < winners.Count; i++)
                {
                    Player tied = players[winners[i]];
                    int handIndex = tied.Value.Hand;
                    tied.Equities[handIndex] += tieEquity;
                    tied.TieEquities[handIndex] += tieEquity;
                }
            }

            // Negative (Loss)
            int winnersIndex = 0;

            for (int i = 0; i < players.Count; i++)
            {
                if (winnersIndex < winners.Count && i == winners[winnersIndex])
                {
                    winnersIndex++;
                    continue;
                }

                Player loser = players[i];
                int handIndex = loser.Value.Hand;
                loser.NegativeEquities[handIndex] += 1;
            }
        }
    }
}
