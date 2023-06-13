using System;

namespace BerldPokerEngine.Poker
{
    internal static class Engine
    {
        private static List<double[]> GetEmptyEquities(int playerAmount)
        {
            List<double[]> equities = new();

            for (int i = 0; i < playerAmount; i++)
            {
                equities.Add(new double[10]);
            }

            return equities;
        }

        private static List<Card> GetAllCards()
        {
            List<Card> cards = new();

            for (int i = 0; i < 52; i++)
            {
                cards.Add(new Card(i));
            }

            return cards;
        }

        internal static List<double[]> Evaluate_5_0(List<List<Card>> holeCards)
        {
            int playerAmount = holeCards.Count;
            List<double[]> equities = GetEmptyEquities(playerAmount);

            List<Card> deadCards = holeCards.Aggregate((a, b) => Enumerable.Concat(a, b).ToList());
            List<Card> aliveCards = GetAllCards().Except(deadCards).ToList();

            HandValue[] handValues = new HandValue[playerAmount];

            for (int i = 0; i < playerAmount; i++)
            {
                handValues[i] = new();
            }

            Card[] cardsToEvaluate = new Card[7];

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

                                for (int i = 0; i < playerAmount; i++)
                                {
                                    cardsToEvaluate[0] = holeCards[i][0];
                                    cardsToEvaluate[1] = holeCards[i][1];

                                    EvaluateCards(cardsToEvaluate, ref handValues[i]);
                                }

                                List<int> winners = new() { 0 };

                                for (int i = 1; i < playerAmount; i++)
                                {
                                    HandValue value = handValues[i];
                                    HandValue winnerValue = handValues[winners[0]];

                                    int comparison = value.Hand - winnerValue.Hand;

                                    if (comparison == 0)
                                    {
                                        for (int j = value.Ranks.Length - 1; j >= 0; j--)
                                        {
                                            if (value.Ranks[j] < 0)
                                            {
                                                break;
                                            }

                                            comparison = value.Ranks[j] - winnerValue.Ranks[j];

                                            if (comparison != 0)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (comparison > 0)
                                    {
                                        winners = new() { i };
                                    }
                                    else if (comparison == 0)
                                    {
                                        winners.Add(i);
                                    }
                                }

                                double winnerEquity = 1.0 / winners.Count;

                                for (int i = 0; i < winners.Count; i++)
                                {
                                    int winnerIndex = winners[i];
                                    int handIndex = (int)handValues[winnerIndex].Hand;
                                    equities[winnerIndex][handIndex] += winnerEquity;
                                }
                            }
                        }
                    }
                }
            }

            return equities;
        }

        private static void EvaluateCards(Card[] cards, ref HandValue handValue)
        {
            // Straight flush
            int[] suitAmount = new int[4];
            int? flushSuit = null;

            for (int i = 0; i < cards.Length; i++)
            {
                Card card = cards[i];
                int suitIndex = card.Suit;
                suitAmount[suitIndex]++;

                if (suitAmount[suitIndex] == 5)
                {
                    flushSuit = card.Suit;
                }
            }

            if (flushSuit.HasValue)
            {
                bool[] coveredFlushRanks = new bool[13];

                for (int i = 0; i < cards.Length; i++)
                {
                    Card card = cards[i];

                    if (card.Suit == flushSuit.Value)
                    {
                        coveredFlushRanks[card.Rank] = true;
                    }
                }

                int consecutiveFlushAmount = 0;

                for (int i = 12; i >= 0; i--)
                {
                    if (coveredFlushRanks[i])
                    {
                        consecutiveFlushAmount++;

                        if (consecutiveFlushAmount == 5)
                        {
                            handValue.Hand = i == Rank.Ten ? Hand.RoyalFlush : Hand.StraightFlush;
                            handValue.Ranks[4] = i + 4;
                            handValue.Ranks[3] = -1;
                            return;
                        }
                        else if (consecutiveFlushAmount == 4 && i == Rank.Deuce && coveredFlushRanks[Rank.Ace])
                        {
                            handValue.Hand = Hand.StraightFlush;
                            handValue.Ranks[4] = Rank.Five;
                            handValue.Ranks[3] = -1;
                            return;
                        }
                    }
                    else
                    {
                        consecutiveFlushAmount = 0;
                    }
                }
            }

            int[] rankAmounts = new int[13];

            for (int i = 0; i < cards.Length; i++)
            {
                rankAmounts[(int)cards[i].Rank]++;
            }

            // Four of a kind
            for (int i = 12; i >= 0; i--)
            {
                if (rankAmounts[i] == 4)
                {
                    for (int j = 12; j >= 0; j--)
                    {
                        if (rankAmounts[j] > 0 && j != i)
                        {
                            handValue.Hand = Hand.FourOfAKind;
                            handValue.Ranks[4] = i;
                            handValue.Ranks[3] = j;
                            handValue.Ranks[2] = -1;
                            return;
                        }
                    }
                }
            }

            // Full house
            int? threeOfAKindRank = null;

            for (int i = 12; i >= 0; i--)
            {
                if (rankAmounts[i] == 3)
                {
                    threeOfAKindRank = i;
                    break;
                }
            }

            if (threeOfAKindRank.HasValue)
            {
                for (int i = 12; i >= 0; i--)
                {
                    if (rankAmounts[i] >= 2 && i != (int)threeOfAKindRank)
                    {
                        handValue.Hand = Hand.FullHouse;
                        handValue.Ranks[4] = threeOfAKindRank.Value;
                        handValue.Ranks[3] = i;
                        handValue.Ranks[2] = -1;
                        return;
                    }
                }
            }

            // Flush
            if (flushSuit.HasValue)
            {
                bool[] coveredFlushRanks = new bool[13];

                for (int i = 0; i < cards.Length; i++)
                {
                    Card card = cards[i];

                    if (card.Suit == flushSuit.Value)
                    {
                        coveredFlushRanks[card.Rank] = true;
                    }
                }

                handValue.Hand = Hand.Flush;
                int ranksIndex = 4;

                for (int i = 12; i >= 0; i--)
                {
                    if (coveredFlushRanks[i])
                    {
                        handValue.Ranks[ranksIndex] = i;
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

            for (int i = 12; i >= 0; i--)
            {
                if (rankAmounts[i] > 0)
                {
                    consecutiveAmount++;

                    if (consecutiveAmount == 5)
                    {
                        handValue.Hand = Hand.Straight;
                        handValue.Ranks[4] = i + 4;
                        handValue.Ranks[3] = -1;
                        return;
                    }
                    else if (consecutiveAmount == 4 && i == Rank.Deuce && rankAmounts[Rank.Ace] > 0)
                    {
                        handValue.Hand = Hand.Straight;
                        handValue.Ranks[4] = Rank.Five;
                        handValue.Ranks[3] = -1;
                        return;
                    }
                }
                else
                {
                    consecutiveAmount = 0;
                }
            }

            // Three of a kind
            if (threeOfAKindRank.HasValue)
            {
                handValue.Hand = Hand.ThreeOfAKind;
                handValue.Ranks[4] = threeOfAKindRank.Value;
                handValue.Ranks[1] = -1;

                int ranksIndex = 3;

                for (int i = 12; i >= 0; i--)
                {
                    if (rankAmounts[i] > 0 && i != (int)threeOfAKindRank)
                    {
                        handValue.Ranks[ranksIndex] = i;
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

            for (int i = 12; i >= 0; i--)
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

                for (int i = 12; i >= 0; i--)
                {
                    if (rankAmounts[i] == 2 && i != highestPairRank.Value)
                    {
                        secondPairRank = i;
                        break;
                    }
                }

                if (secondPairRank.HasValue)
                {
                    for (int i = 12; i >= 0; i--)
                    {
                        if (rankAmounts[i] > 0 && i != highestPairRank.Value && i != secondPairRank.Value)
                        {
                            handValue.Hand = Hand.TwoPair;
                            handValue.Ranks[4] = highestPairRank.Value;
                            handValue.Ranks[3] = secondPairRank.Value;
                            handValue.Ranks[2] = i;
                            handValue.Ranks[1] = -1;
                            return;
                        }
                    }
                }
            }

            // Pair
            if (highestPairRank.HasValue)
            {
                handValue.Hand = Hand.Pair;
                handValue.Ranks[4] = highestPairRank.Value;
                handValue.Ranks[0] = -1;

                int ranksIndex = 3;

                for (int i = 12; i >= 0; i--)
                {
                    if (rankAmounts[i] > 0 && i != highestPairRank.Value)
                    {
                        handValue.Ranks[ranksIndex] = i;
                        ranksIndex--;

                        if (ranksIndex < 1)
                        {
                            break;
                        }
                    }
                }

                return;
            }
            else
            {
                // High card
                handValue.Hand = Hand.HighCard;
                int ranksIndex = 4;

                for (int i = 12; i >= 0; i--)
                {
                    if (rankAmounts[i] > 0)
                    {
                        handValue.Ranks[ranksIndex] = i;
                        ranksIndex--;

                        if (ranksIndex < 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
