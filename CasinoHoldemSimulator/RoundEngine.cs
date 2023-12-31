﻿using BerldPokerEngine;
using BerldPokerEngine.Poker;
using System.Diagnostics;

namespace CasinoHoldemSimulator
{
    internal static class RoundEngine
    {
        internal const int RoundIterationAmount = 1_070_190;
        internal const int FoldWinnings = -(Ante * RoundIterationAmount);

        internal const int PlayerCardAmount = 2;
        internal const int FlopCardAmount = 3;

        private const int Ante = 1;
        private const int ContinueBet = Ante * 2;
        private const int HandValueCardAmount = 7;

        internal static int[] EvaluateRound(List<Card> playerCards, List<Card> flopCards)
        {
            IEnumerable<Card> deadCards = Enumerable.Concat(playerCards, flopCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            Card[] playerAllCards = new Card[HandValueCardAmount];
            HandValue playerValue = new();

            Card[] dealerAllCards = new Card[HandValueCardAmount];
            HandValue dealerValue = new();

            playerAllCards[0] = playerCards[0];
            playerAllCards[1] = playerCards[1];

            for (int i = 0; i < flopCards.Count; i++)
            {
                playerAllCards[PlayerCardAmount + i] = flopCards[i];
                dealerAllCards[PlayerCardAmount + i] = flopCards[i];
            }

            int[] winnings = new int[WinningKind.Amount];

            for (int b4 = 0; b4 < aliveCards.Length; b4++)
            {
                for (int b5 = b4 + 1; b5 < aliveCards.Length; b5++)
                {
                    for (int d1 = 0; d1 < aliveCards.Length; d1++)
                    {
                        if (d1 == b4 || d1 == b5) continue;

                        for (int d2 = d1 + 1; d2 < aliveCards.Length; d2++)
                        {
                            if (d2 == b4 || d2 == b5) continue;

                            dealerAllCards[0] = aliveCards[d1];
                            dealerAllCards[1] = aliveCards[d2];

                            playerAllCards[5] = aliveCards[b4];
                            playerAllCards[6] = aliveCards[b5];

                            dealerAllCards[5] = aliveCards[b4];
                            dealerAllCards[6] = aliveCards[b5];

                            Engine.SetHandValue(playerAllCards, playerValue);
                            Engine.SetHandValue(dealerAllCards, dealerValue);

                            bool dealerQualifies = dealerValue.Hand > Hand.Pair ||
                                (dealerValue.Hand == Hand.Pair &&
                                dealerValue.Ranks[HandValue.HandRankIndex] >= Rank.Four);

                            if (dealerQualifies)
                            {
                                int comparison = playerValue.CompareTo(dealerValue);

                                if (comparison > 0)
                                {
                                    winnings[WinningKind.WinContinueBet] += ContinueBet;

                                    int winningKind = GetWinningKind(playerValue);
                                    winnings[winningKind] += Ante * GetAnteMultiplier(playerValue);
                                }
                                else if (comparison < 0)
                                {
                                    winnings[WinningKind.LossContinue] -= ContinueBet;
                                    winnings[WinningKind.LossContinue] -= Ante;
                                }
                            }
                            else
                            {
                                int winningKind = GetWinningKind(playerValue);
                                winnings[winningKind] += Ante * GetAnteMultiplier(playerValue);
                            }
                        }
                    }
                }
            }

            return winnings;
        }

        internal static List<NormalRound> GetNormalRounds()
        {
            int bound = EngineData.AllCardsAmount;
            Dictionary<string, NormalRound> normalRoundMap = new();

            for (int p1 = 0; p1 < bound; p1++)
            {
                for (int p2 = p1 + 1; p2 < bound; p2++)
                {
                    for (int f1 = 0; f1 < bound; f1++)
                    {
                        if (f1 == p1 || f1 == p2) continue;

                        for (int f2 = f1 + 1; f2 < bound; f2++)
                        {
                            if (f2 == p1 || f2 == p2) continue;

                            for (int f3 = f2 + 1; f3 < bound; f3++)
                            {
                                if (f3 == p1 || f3 == p2) continue;

                                NormalRound round = new(p1, p2, f1, f2, f3);

                                if (normalRoundMap.ContainsKey(round.Key))
                                {
                                    normalRoundMap[round.Key].Frequency++;
                                }
                                else
                                {
                                    normalRoundMap.Add(round.Key, round);
                                }
                            }
                        }
                    }
                }
            }

            return normalRoundMap.Values.ToList();
        }

        private static int GetWinningKind(HandValue value)
        {
            return value.Hand switch
            {
                Hand.RoyalFlush => WinningKind.WinAnteRoyalFlush,
                Hand.StraightFlush => WinningKind.WinAnteStraightFlush,
                Hand.FourOfAKind => WinningKind.WinAnteFourOfAKind,
                Hand.FullHouse => WinningKind.WinAnteFullHouse,
                Hand.Flush => WinningKind.WinAnteFlush,
                _ => WinningKind.WinAnteDefault
            };
        }

        private static int GetAnteMultiplier(HandValue value)
        {
            return value.Hand switch
            {
                Hand.RoyalFlush => 100,
                Hand.StraightFlush => 20,
                Hand.FourOfAKind => 10,
                Hand.FullHouse => 3,
                Hand.Flush => 2,
                _ => 1
            };
        }
    }
}
