using BerldPokerEngine.Poker;
using BerldPokerEngine;
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
        private const int LastHandRankIndex = 4;

        private const int HandValueCardAmount = 7;

        internal static int EvaluateRound(List<Card> playerCards, List<Card> flopCards)
        {
            Debug.Assert(playerCards.Count == PlayerCardAmount);
            Debug.Assert(flopCards.Count == FlopCardAmount);

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

            int roundWinnings = 0;

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
                                dealerValue.Ranks[LastHandRankIndex] >= Rank.Four);

                            if (dealerQualifies)
                            {
                                int comparison = playerValue.CompareTo(dealerValue);

                                if (comparison > 0)
                                {
                                    roundWinnings += ContinueBet;
                                    roundWinnings += Ante * GetAnteMultiplier(playerValue);
                                }
                                else if (comparison < 0)
                                {
                                    roundWinnings -= ContinueBet;
                                    roundWinnings -= Ante;
                                }
                            }
                            else
                            {
                                roundWinnings += Ante * GetAnteMultiplier(playerValue);
                            }
                        }
                    }
                }
            }

            return roundWinnings;
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
