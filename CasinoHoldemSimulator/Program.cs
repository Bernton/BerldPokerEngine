using BerldPokerEngine.Poker;

namespace CasinoHoldemSimulator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 1 && args.First().Length == 11)
            {
                string input = args.First();
                string playerInput = input[..4];
                string flopInput = input[5..];

                List<Card> playerCards = InputToCards(playerInput);
                List<Card> flopCards = InputToCards(flopInput);
                IEnumerable<Card> allCards = Enumerable.Concat(playerCards, flopCards);

                if (allCards.Distinct().Count() != allCards.Count())
                {
                    Console.Error.WriteLine("Duplicate card input.");
                    Environment.Exit(1);
                }

                int roundWinnings = RoundEngine.EvaluateRound(playerCards, flopCards);

                bool shouldFold = RoundEngine.FoldWinnings > roundWinnings;
                string action = shouldFold ? "Fold" : "Continue";
                double evRatio = roundWinnings / (double)RoundEngine.RoundIterationAmount;
                string signText = evRatio > 0 ? "win" : "loss";

                Console.WriteLine($"{action} [Average {signText} of {Math.Abs(evRatio):0.00} times the ante]");
                Console.WriteLine();
            }
            else if (args.Length == 0)
            {
                DateTime startTime = DateTime.Now;

                int roundAmount = 1_000_000;
                int roundsFolded = 0;
                long totalWinnings = 0;

                Deck deck = new();
                List<Card> playerCards = new() { default, default };
                List<Card> flopCards = new() { default, default, default };

                for (int u = 0; u < roundAmount; u++)
                {
                    if (u % 100 == 0 && u > 0)
                    {
                        WriteStatus(u, roundsFolded, totalWinnings, DateTime.Now - startTime);
                    }

                    deck.Reset();

                    for (int i = 0; i < RoundEngine.PlayerCardAmount; i++)
                    {
                        playerCards[i] = deck.Draw();
                    }

                    for (int i = 0; i < RoundEngine.FlopCardAmount; i++)
                    {
                        flopCards[i] = deck.Draw();
                    }

                    int roundWinnings = RoundEngine.EvaluateRound(playerCards, flopCards);

                    bool shouldFold = RoundEngine.FoldWinnings > roundWinnings;

                    if (shouldFold)
                    {
                        roundsFolded++;
                        totalWinnings += RoundEngine.FoldWinnings;
                    }
                    else
                    {
                        totalWinnings += roundWinnings;
                    }
                }

                WriteStatus(roundAmount, roundsFolded, totalWinnings, DateTime.Now - startTime);
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

        private static void WriteStatus(int roundAmount, int roundsFolded, long totalWinnings, TimeSpan elapsed)
        {
            long iterationsDone = roundAmount * (long)RoundEngine.RoundIterationAmount;
            double netCentWinningsPerIteration = totalWinnings / (double)iterationsDone * 100;
            double roundsPerSecond = roundAmount / elapsed.TotalSeconds;

            Console.WriteLine($"Average of {netCentWinningsPerIteration:0.000}¢ winnings per iteration");
            Console.WriteLine($"Folded {roundsFolded} of {roundAmount} rounds");
            Console.WriteLine($"At {roundsPerSecond:0.00} rounds per second");
            Console.WriteLine();
        }
    }
}