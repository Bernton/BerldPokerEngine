using BerldPokerEngine;
using BerldPokerEngine.Poker;
using System.Diagnostics;

namespace CasinoHoldemSimulator
{
    internal class Program
    {
        private const int EvaluateIterationAmount = 1_070_190;
        private const int Ante = 1;
        private const int ContinueBet = Ante * 2;
        private const int FoldWinnings = -(Ante * EvaluateIterationAmount);
        private const int LastHandRankIndex = 4;
        private const int PlayerCardAmount = 2;
        private const int FlopCardAmount = 3;

        private static int Evaluate(List<Card> playerCards, List<Card> flopCards)
        {
            Debug.Assert(playerCards.Count == PlayerCardAmount);
            Debug.Assert(flopCards.Count == FlopCardAmount);

            List<Card> deadCards = Enumerable.Concat(playerCards, flopCards).ToList();
            List<Card> aliveCards = EngineData.GetAllCards().Except(deadCards).ToList();

            Card[] playerAllCards = new Card[7];
            HandValue playerValue = new();

            Card[] dealerAllCards = new Card[7];
            HandValue dealerValue = new();

            playerAllCards[0] = playerCards[0];
            playerAllCards[1] = playerCards[1];

            for (int i = 0; i < flopCards.Count; i++)
            {
                playerAllCards[PlayerCardAmount + i] = flopCards[i];
                dealerAllCards[PlayerCardAmount + i] = flopCards[i];
            }

            int evaluationWinnings = 0;

            for (int b4 = 0; b4 < aliveCards.Count; b4++)
            {
                for (int b5 = b4 + 1; b5 < aliveCards.Count; b5++)
                {
                    for (int d1 = 0; d1 < aliveCards.Count; d1++)
                    {
                        if (d1 == b4 || d1 == b5) continue;

                        for (int d2 = d1 + 1; d2 < aliveCards.Count; d2++)
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
                                    evaluationWinnings += ContinueBet;
                                    evaluationWinnings += Ante * GetMultiplier(playerValue);
                                }
                                else if (comparison < 0)
                                {
                                    evaluationWinnings -= ContinueBet;
                                    evaluationWinnings -= Ante;
                                }
                            }
                            else
                            {
                                evaluationWinnings += Ante * GetMultiplier(playerValue);
                            }
                        }
                    }
                }
            }

            return evaluationWinnings;
        }

        private static void Main(string[] args)
        {
            if (args.Length == 1 && args.First().Length == 11)
            {
                string input = args.First();
                string playerInput = input[..4];
                string flopInput = input[5..];

                List<Card> playerCards = InputToCards(playerInput);
                List<Card> flopCards = InputToCards(flopInput);
                List<Card> allCards = Enumerable.Concat(playerCards, flopCards).ToList();

                if (allCards.Distinct().Count() != allCards.Count)
                {
                    Console.Error.WriteLine("Duplicate card input.");
                    Environment.Exit(1);
                }

                int evaluationWinnings = Evaluate(playerCards, flopCards);

                bool shouldFold = FoldWinnings > evaluationWinnings;
                string action = shouldFold ? "Fold" : "Continue";
                double evRatio = evaluationWinnings / (double)EvaluateIterationAmount;
                string signText = evRatio > 0 ? "win" : "loss";

                Console.WriteLine($"{action} [Average {signText} of {Math.Abs(evRatio):0.00} times the ante]");
                Console.WriteLine();
            }
            else if (args.Length == 0)
            {
                Deck deck = new();

                int evaluationAmount = 10_000;
                int evaluationsFoldedAmount = 0;
                long totalWinnings = 0;

                List<Card> playerCards = new() { default, default };
                List<Card> flopCards = new() { default, default, default };

                for (int u = 0; u < evaluationAmount; u++)
                {
                    if (u > 0 && u % 100 == 0)
                    {
                        int evaluationsDone = u;
                        long iterationsDone = evaluationsDone * (long)EvaluateIterationAmount;
                        double netCentWinningsPerIteration = totalWinnings / (double)iterationsDone * 100;

                        Console.WriteLine($"Average of {netCentWinningsPerIteration:0.000}¢ winnings per iteration");
                        Console.WriteLine($"Folded {evaluationsFoldedAmount} of {evaluationsDone} rounds.");
                        Console.WriteLine();
                    }

                    deck.Reset();

                    for (int i = 0; i < 2; i++)
                    {
                        playerCards[i] = deck.Draw();
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        flopCards[i] = deck.Draw();
                    }

                    int evaluationWinnings = Evaluate(playerCards, flopCards);

                    bool shouldFold = FoldWinnings > evaluationWinnings;

                    if (shouldFold)
                    {
                        evaluationsFoldedAmount++;
                        totalWinnings += FoldWinnings;
                    }
                    else
                    {
                        totalWinnings += evaluationWinnings;
                    }
                }
            }
        }

        private static List<Card> InputToCards(string input)
        {
            int cardAmount = input.Length / 2;
            List<Card> cards = new();

            for (int i = 0; i < cardAmount; i++)
            {
                char rankChar = input[i * 2];
                char suitChar = input[i * 2 + 1];

                if (rankChar == 'X' && suitChar == 'x')
                {
                    continue;
                }

                int? rank = Rank.FromChar(rankChar);
                int? suit = Suit.FromChar(suitChar);

                if (rank.HasValue && suit.HasValue)
                {
                    cards.Add(Card.Create(rank.Value, suit.Value));
                }
                else
                {
                    Console.Error.WriteLine("Invalid card input.");
                    Environment.Exit(1);
                }
            }

            return cards;
        }

        private static int GetMultiplier(HandValue value)
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