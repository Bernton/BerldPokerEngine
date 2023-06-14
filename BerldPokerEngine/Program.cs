using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new[] { "XxXxXxXxXx 2c2d XxXx" };
            }

            Evaluate(args);
        }

        private static void Evaluate(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("There must be 1 argument.");
                Environment.Exit(1);
            }

            string input = args[0];

            if (input.Length < 15)
            {
                Console.Error.WriteLine("Input must have at least length 15.");
                Environment.Exit(1);
            }

            if (input.Length % 5 != 0)
            {
                Console.Error.WriteLine("Input length is not valid.");
                Environment.Exit(1);
            }

            string boardInput = input[..10];

            List<Card> boardCards = InputToCards(boardInput);

            string holeCardInput = input[10..];

            List<string> playerCardInputs = new();
            List<List<Card>> holeCards = new();

            int playerAmount = holeCardInput.Length / 5;

            for (int i = 0; i < playerAmount; i++)
            {
                string playerCardInput = holeCardInput.Substring(i * 5 + 1, 4);
                List<Card> playerCards = InputToCards(playerCardInput);

                playerCardInputs.Add(playerCardInput);
                holeCards.Add(playerCards);
            }

            DateTime startTime = DateTime.Now;

            List<Player>? playerStats = Engine.Evaluate(boardCards, holeCards);

            if (playerStats is null)
            {
                Console.Error.WriteLine("Format not supported.");
                Environment.Exit(1);
            }

            playerStats = playerStats.OrderBy(c => c.Index).ToList();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsed = endTime - startTime;

            double totalEquity = playerStats.Sum(c => c.Equities.Sum());
            double equityPerMillisecond = totalEquity / elapsed.TotalMilliseconds;

            Console.WriteLine($"Time: {elapsed.TotalMilliseconds:0.0} ms");
            Console.WriteLine($"Speed: {equityPerMillisecond:0} equity/ms");
            Console.WriteLine($"Total:\t\t\t{totalEquity,15:0.0} {100.0,14:0.00000000}%");
            Console.WriteLine();

            for (int i = 0; i < playerStats.Count; i++)
            {
                double[] playerEquity = playerStats[i].Equities;
                double totalPlayerEquity = playerEquity.Sum();
                double totalPlayerEquityPercent = totalPlayerEquity / totalEquity * 100;

                Console.WriteLine($"Player {(i + 1)} - {playerCardInputs[i]}");
                Console.WriteLine($"Equity:\t\t\t{totalPlayerEquity,15:0.0} {totalPlayerEquityPercent,14:0.00000000}%");

                for (int j = 0; j < Hand.Amount; j++)
                {
                    int hand = j;
                    double handEquity = playerEquity[j];
                    double handEquityPercent = handEquity / totalEquity * 100;
                    string caption = Hand.ToFormatString(hand);
                    string padding = Hand.GetTabPadding(hand);

                    Console.WriteLine($"{caption}:{padding}{handEquity,15:0.0} {handEquityPercent,14:0.00000000}%");
                }

                Console.WriteLine();
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
                    cards.Add(new(rank.Value, suit.Value));
                }
                else
                {
                    Console.Error.WriteLine("Invalid card input.");
                    Environment.Exit(1);
                }
            }

            return cards;
        }
    }
}