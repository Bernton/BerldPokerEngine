using BerldPokerEngine.Poker;
using BerldPokerEngine;
using static TexasHoldemBonusSimulator.Engines.EngineHelper;

namespace TexasHoldemBonusSimulator.Engines
{
    internal static class DecisionEngine
    {
        internal static bool EvaluateFlop(List<Card> playerCards, List<Card> boardCards)
        {
            const int IterationAmount = 1_070_190;
            const int WinScoreThreshold = IterationAmount;

            IEnumerable<Card> deadCards = playerCards.Concat(boardCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            Card[] playerAllCards = new Card[HandValueCardAmount];
            HandValue playerValue = new();

            Card[] dealerAllCards = new Card[HandValueCardAmount];
            HandValue dealerValue = new();

            playerAllCards[0] = playerCards[0];
            playerAllCards[1] = playerCards[1];

            for (int i = 0; i < boardCards.Count; i++)
            {
                playerAllCards[PlayerCardAmount + i] = boardCards[i];
                dealerAllCards[PlayerCardAmount + i] = boardCards[i];
            }

            int iterationsLeft = IterationAmount;
            int winScore = 0;

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

                            int comparison = playerValue.CompareTo(dealerValue);

                            if (comparison > 0)
                            {
                                winScore += 2;
                            }
                            else if (comparison == 0)
                            {
                                winScore += 1;
                            }

                            if (winScore > WinScoreThreshold)
                            {
                                return true;
                            }
                            else if (winScore + iterationsLeft * 2 <= WinScoreThreshold)
                            {
                                return false;
                            }

                            iterationsLeft--;
                        }
                    }
                }
            }

            return false;
        }

        internal static bool EvaluateTurn(List<Card> playerCards, List<Card> boardCards)
        {
            const int IterationAmount = 45_540;
            const int WinScoreThreshold = IterationAmount;

            IEnumerable<Card> deadCards = playerCards.Concat(boardCards);
            Card[] aliveCards = EngineData.GetAllCards().Except(deadCards).ToArray();

            Card[] playerAllCards = new Card[HandValueCardAmount];
            HandValue playerValue = new();

            Card[] dealerAllCards = new Card[HandValueCardAmount];
            HandValue dealerValue = new();

            playerAllCards[0] = playerCards[0];
            playerAllCards[1] = playerCards[1];

            for (int i = 0; i < boardCards.Count; i++)
            {
                playerAllCards[PlayerCardAmount + i] = boardCards[i];
                dealerAllCards[PlayerCardAmount + i] = boardCards[i];
            }

            int iterationsLeft = IterationAmount;
            int winScore = 0;

            for (int b5 = 0; b5 < aliveCards.Length; b5++)
            {
                for (int d1 = 0; d1 < aliveCards.Length; d1++)
                {
                    if (d1 == b5) continue;

                    for (int d2 = d1 + 1; d2 < aliveCards.Length; d2++)
                    {
                        if (d2 == b5) continue;

                        dealerAllCards[0] = aliveCards[d1];
                        dealerAllCards[1] = aliveCards[d2];

                        playerAllCards[6] = aliveCards[b5];
                        dealerAllCards[6] = aliveCards[b5];

                        Engine.SetHandValue(playerAllCards, playerValue);
                        Engine.SetHandValue(dealerAllCards, dealerValue);

                        int comparison = playerValue.CompareTo(dealerValue);

                        if (comparison > 0)
                        {
                            winScore += 2;
                        }
                        else if (comparison == 0)
                        {
                            winScore += 1;
                        }

                        if (winScore > WinScoreThreshold)
                        {
                            return true;
                        }
                        else if (winScore + iterationsLeft * 2 <= WinScoreThreshold)
                        {
                            return false;
                        }

                        iterationsLeft--;
                    }
                }
            }

            return false;
        }
    }
}
