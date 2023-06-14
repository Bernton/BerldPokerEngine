namespace BerldPokerEngine.Poker
{
    internal static class Engine
    {
        internal static List<Player>? Evaluate(List<Card> boardCards, List<List<Card>> holeCards)
        {
            List<Player> players = EngineHelpers.GetPlayersFromHoleCards(holeCards);
            players = players.OrderByDescending(c => c.WildCardAmount).ToList();

            int wildBoardCardAmount = 5 - boardCards.Count;
            int wildPlayerCardAmount = players.Count * 2 - players.Sum(c => c.HoleCards.Count);

            if (wildBoardCardAmount == 5)
            {
                if (wildPlayerCardAmount == 0)
                {
                    return Evaluate_5_0(ref players);
                }

                if (players.Count >= 2 &&
                    players[0].WildCardAmount == 2 &&
                    players[1].WildCardAmount == 0)
                {
                    return Evaluate_5_2(ref players);
                }
            }

            return null;
        }

        private static List<Player>? Evaluate_5_2(ref List<Player> players)
        {
            List<Card> aliveCards = EngineHelpers.GetAliveCards(players);

            Card[] cardsToEvaluate = new Card[7];
            List<int> winners = new();

            for (int playerCardI1 = 0; playerCardI1 < aliveCards.Count; playerCardI1++)
            {
                for (int playerCardI2 = playerCardI1 + 1; playerCardI2 < aliveCards.Count; playerCardI2++)
                {
                    for (int boardCardI1 = 0; boardCardI1 < aliveCards.Count; boardCardI1++)
                    {
                        if (boardCardI1 == playerCardI1 || boardCardI1 == playerCardI2) continue;
                        for (int boardCardI2 = boardCardI1 + 1; boardCardI2 < aliveCards.Count; boardCardI2++)
                        {
                            if (boardCardI2 == playerCardI1 || boardCardI2 == playerCardI2) continue;
                            for (int boardCardI3 = boardCardI2 + 1; boardCardI3 < aliveCards.Count; boardCardI3++)
                            {
                                if (boardCardI3 == playerCardI1 || boardCardI3 == playerCardI2) continue;
                                for (int boardCardI4 = boardCardI3 + 1; boardCardI4 < aliveCards.Count; boardCardI4++)
                                {
                                    if (boardCardI4 == playerCardI1 || boardCardI4 == playerCardI2) continue;
                                    for (int boardCardI5 = boardCardI4 + 1; boardCardI5 < aliveCards.Count; boardCardI5++)
                                    {
                                        if (boardCardI5 == playerCardI1 || boardCardI5 == playerCardI2) continue;

                                        cardsToEvaluate[2] = aliveCards[boardCardI1];
                                        cardsToEvaluate[3] = aliveCards[boardCardI2];
                                        cardsToEvaluate[4] = aliveCards[boardCardI3];
                                        cardsToEvaluate[5] = aliveCards[boardCardI4];
                                        cardsToEvaluate[6] = aliveCards[boardCardI5];

                                        for (int i = 0; i < players.Count; i++)
                                        {
                                            Player player = players[i];

                                            if (i == 0)
                                            {
                                                cardsToEvaluate[0] = aliveCards[playerCardI1];
                                                cardsToEvaluate[1] = aliveCards[playerCardI2];
                                            }
                                            else
                                            {
                                                cardsToEvaluate[0] = player.HoleCards[0];
                                                cardsToEvaluate[1] = player.HoleCards[1];
                                            }

                                            EvaluateCards(cardsToEvaluate, ref player);
                                        }

                                        AddEquityToWinners(ref players, ref winners);
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(playerCardI1);
            }

            return players;
        }

        internal static List<Player> Evaluate_5_0(ref List<Player> players)
        {
            List<Card> aliveCards = EngineHelpers.GetAliveCards(players);

            Card[] cardsToEvaluate = new Card[7];
            List<int> winners = new();

            for (int boardCardI1 = 0; boardCardI1 < aliveCards.Count; boardCardI1++)
            {
                for (int boardCardI2 = boardCardI1 + 1; boardCardI2 < aliveCards.Count; boardCardI2++)
                {
                    for (int boardCardI3 = boardCardI2 + 1; boardCardI3 < aliveCards.Count; boardCardI3++)
                    {
                        for (int boardCardI4 = boardCardI3 + 1; boardCardI4 < aliveCards.Count; boardCardI4++)
                        {
                            for (int boardCardI5 = boardCardI4 + 1; boardCardI5 < aliveCards.Count; boardCardI5++)
                            {
                                cardsToEvaluate[2] = aliveCards[boardCardI1];
                                cardsToEvaluate[3] = aliveCards[boardCardI2];
                                cardsToEvaluate[4] = aliveCards[boardCardI3];
                                cardsToEvaluate[5] = aliveCards[boardCardI4];
                                cardsToEvaluate[6] = aliveCards[boardCardI5];

                                for (int i = 0; i < players.Count; i++)
                                {
                                    Player player = players[i];

                                    cardsToEvaluate[0] = player.HoleCards[0];
                                    cardsToEvaluate[1] = player.HoleCards[1];

                                    EvaluateCards(cardsToEvaluate, ref player);
                                }

                                AddEquityToWinners(ref players, ref winners);
                            }
                        }
                    }
                }
            }

            return players;
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
