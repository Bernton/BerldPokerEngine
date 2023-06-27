using BerldPokerEngine;
using BerldPokerEngine.Poker;

namespace CasinoHoldemSimulator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int lastRankI = 4;

            Card[] playerAllCards = new Card[7];
            Card[] dealerAllCards = new Card[7];

            HandValue playerValue = new();
            HandValue dealerValue = new();

            if (args.Length == 1 && args.First().Length == 11)
            {
                string input = args.First();
                string playerInput = input[..4];
                string flopInput = input[5..];

                List<Card> playerCards = InputToCards(playerInput);
                List<Card> flopCards = InputToCards(flopInput);
                List<Card> deadCards = Enumerable.Concat(playerCards, flopCards).ToList();

                if (deadCards.Distinct().Count() != deadCards.Count)
                {
                    Console.Error.WriteLine("Duplicate card input.");
                    Environment.Exit(1);
                }

                for (int i = 0; i < flopCards.Count; i++)
                {
                    playerAllCards[i] = flopCards[i];
                    dealerAllCards[i] = flopCards[i];
                }

                playerAllCards[5] = playerCards[0];
                playerAllCards[6] = playerCards[1];

                Deck deck = new();

                foreach (Card card in deadCards)
                {
                    deck.Draw(card);
                }

                deck.Snapshot();

                int iterationAmount = 1_000_000;
                int iterationWinnings = 0;
                int ante = 1;
                int bet = ante * 2;

                for (int u = 0; u < iterationAmount; u++)
                {
                    deck.Restore();

                    dealerAllCards[5] = deck.Draw();
                    dealerAllCards[6] = deck.Draw();

                    for (int i = 3; i < 5; i++)
                    {
                        Card boardCard = deck.Draw();
                        playerAllCards[i] = boardCard;
                        dealerAllCards[i] = boardCard;
                    }

                    Engine.SetHandValue(playerAllCards, playerValue);
                    Engine.SetHandValue(dealerAllCards, dealerValue);

                    bool dealerDidNotQualify = dealerValue.Hand == Hand.HighCard ||
                        (dealerValue.Hand == Hand.Pair &&
                        dealerValue.Ranks[lastRankI] < Rank.Four);

                    if (dealerDidNotQualify)
                    {
                        iterationWinnings += ante * GetMultiplier(playerValue);
                    }
                    else
                    {
                        int comparison = playerValue.CompareTo(dealerValue);

                        if (comparison > 0)
                        {
                            iterationWinnings += bet;
                            iterationWinnings += ante * GetMultiplier(playerValue);
                        }
                        else if (comparison < 0)
                        {
                            iterationWinnings -= bet;
                            iterationWinnings -= ante;
                        }
                    }
                }

                int foldWinnings = -(ante * iterationAmount);
                bool shouldFold = foldWinnings > iterationWinnings;
                string action = shouldFold ? "Fold" : "Continue";
                double evRatio = iterationWinnings / (double)iterationAmount;
                string signText = evRatio > 0 ? "win" : "loss";

                Console.WriteLine($"{action} [Average {signText} of {Math.Abs(evRatio):0.00} times the ante]");
                Console.WriteLine();
            }
            else if (args.Length == 0)
            {
                Deck deck = new();

                int roundAmount = 10_000;
                int roundFoldedAmount = 0;
                int trialsPerRound = roundAmount / 10;
                int iterationAmount = roundAmount * trialsPerRound;


                int initialMoney = iterationAmount / 10;
                int money = initialMoney;
                int ante = 1;
                int bet = 2;

                for (int u = 0; u < roundAmount; u++)
                {
                    deck.Reset();

                    for (int i = 0; i < 3; i++)
                    {
                        Card boardCard = deck.Draw();
                        playerAllCards[i] = boardCard;
                        dealerAllCards[i] = boardCard;
                    }

                    playerAllCards[5] = deck.Draw();
                    playerAllCards[6] = deck.Draw();

                    deck.Snapshot();

                    int trialWinnings = 0;

                    for (int v = 0; v < trialsPerRound; v++)
                    {
                        deck.Restore();

                        dealerAllCards[5] = deck.Draw();
                        dealerAllCards[6] = deck.Draw();

                        for (int i = 3; i < 5; i++)
                        {
                            Card boardCard = deck.Draw();
                            playerAllCards[i] = boardCard;
                            dealerAllCards[i] = boardCard;
                        }

                        Engine.SetHandValue(playerAllCards, playerValue);
                        Engine.SetHandValue(dealerAllCards, dealerValue);

                        bool dealerDidNotQualify = dealerValue.Hand == Hand.HighCard ||
                            (dealerValue.Hand == Hand.Pair &&
                            dealerValue.Ranks[lastRankI] < Rank.Four);

                        if (dealerDidNotQualify)
                        {
                            trialWinnings += ante * GetMultiplier(playerValue);
                        }
                        else
                        {
                            int comparison = playerValue.CompareTo(dealerValue);

                            if (comparison > 0)
                            {
                                trialWinnings += bet;
                                trialWinnings += ante * GetMultiplier(playerValue);
                            }
                            else if (comparison < 0)
                            {
                                trialWinnings -= bet;
                                trialWinnings -= ante;
                            }
                        }
                    }

                    int foldWinnings = -(trialsPerRound * ante);
                    bool shouldFold = foldWinnings > trialWinnings;

                    if (shouldFold)
                    {
                        roundFoldedAmount++;
                        money += foldWinnings;
                    }
                    else
                    {
                        money += trialWinnings;
                    }
                }

                int netWinnings = money - initialMoney;
                double netCentWinningsPerIteration = netWinnings / (double)iterationAmount * 100;

                Console.WriteLine($"Started with {initialMoney}$");
                Console.WriteLine($"After {iterationAmount} iterations left with {money}$");
                Console.WriteLine($"Average of {netCentWinningsPerIteration:0.000}¢ winnings per iteration");
                Console.WriteLine($"Folded {roundFoldedAmount} of {roundAmount} rounds.");
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