using BerldPokerEngine;
using BerldPokerEngine.Poker;

namespace ConsoleAppOutput
{
    internal class Program
    {
        private class StartingHand
        {
            internal double Equity { get; set; }
            internal Card Lower { get; set; }
            internal Card Higher { get; set; }

            public override string ToString()
            {
                string suffix;

                if (Lower.Rank != Higher.Rank)
                {
                    if (Lower.Suit == Higher.Suit)
                    {
                        suffix = "s";
                    }
                    else
                    {
                        suffix = "o";
                    }
                }
                else
                {
                    suffix = string.Empty;
                }

                return $"{Rank.ToChar(Higher.Rank)}{Rank.ToChar(Lower.Rank)}{suffix}";
            }
        }

        private static void Main(string[] args)
        {
            int playerAmount = 3;

            if (args.Length == 0 ||
                !int.TryParse(args[0], out playerAmount)
                || playerAmount < 0 ||
                playerAmount > 23)
            {
                Console.Error.WriteLine("Invalid argument(s).");
                Console.WriteLine("Continue with 3.");
            }

            int opponentAmount = playerAmount - 1;

            List<StartingHand> startingHands = GetStartingHands();
            startingHands = startingHands.OrderBy(c => c.ToString()).ToList();

            Random random = new();

            foreach (StartingHand startingHand in startingHands)
            {
                List<List<Card>?> holeCards = new()
                {
                    new()
                    {
                        startingHand.Lower,
                        startingHand.Higher
                    }
                };

                for (int i = 0; i < opponentAmount; i++)
                {
                    holeCards.Add(null);
                }

                List<Player> players = RandomEngine.Evaluate(null, holeCards, 1_000_000, random.Next);
                startingHand.Equity = players.First().TotalEquity;

                Console.WriteLine($"{startingHand.Equity}");
            }
        }

        private static List<StartingHand> GetStartingHands()
        {
            List<StartingHand> startingHands = new();

            for (int lowerRank = 0; lowerRank < 13; lowerRank++)
            {
                for (int higherRank = lowerRank; higherRank < 13; higherRank++)
                {
                    Card lower = Card.Create(lowerRank, Suit.Clubs);
                    Card higherSuited = Card.Create(higherRank, Suit.Clubs);
                    Card higherOffsuit = Card.Create(higherRank, Suit.Diamonds);

                    if (lowerRank != higherRank)
                    {
                        startingHands.Add(new()
                        {
                            Lower = lower,
                            Higher = higherSuited
                        });
                    }

                    startingHands.Add(new()
                    {
                        Lower = lower,
                        Higher = higherOffsuit
                    });
                }
            }

            return startingHands;
        }
    }
}