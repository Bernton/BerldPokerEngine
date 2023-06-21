using BerldPokerEngine;
using BerldPokerEngine.Poker;

namespace ConsoleClient
{
    internal static class ConsoleHandler
    {
        private const long MaxPermittedIterations = 3_000_000_000L;

        internal static void Evaluate(string[] args)
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
            string holeCardInput = input[10..];

            List<Card> boardCards = InputToCards(boardInput);
            List<Card> allCards = new(boardCards);

            List<string> playerCardInputs = new();
            List<List<Card>?> holeCards = new();

            int playerAmount = holeCardInput.Length / 5;

            for (int i = 0; i < playerAmount; i++)
            {
                string playerCardInput = holeCardInput.Substring(i * 5 + 1, 4);
                List<Card> playerCards = InputToCards(playerCardInput);

                playerCardInputs.Add(playerCardInput);
                holeCards.Add(playerCards);
                allCards.AddRange(playerCards);
            }

            if (allCards.Distinct().Count() != allCards.Count)
            {
                Console.Error.WriteLine("Duplicate card input.");
                Environment.Exit(1);
            }

            long iterations = Engine.CalculateIterationAmount(boardCards, holeCards);

            if (iterations > MaxPermittedIterations)
            {
                Console.Error.WriteLine("Input requires more iterations than permitted.");
                Console.Error.WriteLine($"Required:\t{iterations,25}");
                Console.Error.WriteLine($"Permitted:\t{MaxPermittedIterations,25}");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine($"Iterations: {iterations}");
            }

            DateTime startTime = DateTime.Now;

            List<Player> playerStats = Engine.Evaluate(boardCards, holeCards);

            DateTime endTime = DateTime.Now;
            TimeSpan elapsed = endTime - startTime;

            double totalEquity = playerStats.Sum(c => c.TotalEquity);
            double equityPerMillisecond = totalEquity / elapsed.TotalMilliseconds;

            Console.WriteLine($"Time: {elapsed.TotalMilliseconds:0.0} ms");
            Console.WriteLine($"Speed: {equityPerMillisecond:0} equity/ms");
            Console.WriteLine();
            Console.WriteLine($"Board:     {boardInput}");
            WriteEquityLine("Equity", "\t\t\t", totalEquity, 100.0);
            Console.WriteLine();

            foreach (Player player in playerStats)
            {
                Console.WriteLine($"Player {player.Index + 1} - {playerCardInputs[player.Index]}");

                double totalPlayerEquity = player.TotalEquity;
                double totalPlayerEquityPercent = totalPlayerEquity / totalEquity * 100;
                WriteEquityLine("Equity", "\t\t\t", totalPlayerEquity, totalPlayerEquityPercent);

                //Console.WriteLine("<Equity>");

                for (int hand = 0; hand < Hand.Amount; hand++)
                {
                    string caption = Hand.ToFormatString(hand);
                    string padding = Hand.GetTabPadding(hand);
                    double handEquity = player.EquityAmounts[hand];
                    double handEquityPercent = handEquity / totalEquity * 100;
                    WriteEquityLine(caption, padding, handEquity, handEquityPercent);
                }

                //Console.WriteLine("<Win equity>");

                //for (int hand = 0; hand < Hand.Amount; hand++)
                //{
                //    string caption = Hand.ToFormatString(hand);
                //    string padding = Hand.GetTabPadding(hand);
                //    double handEquity = player.WinEquityAmounts[hand];
                //    double handEquityPercent = handEquity / totalEquity * 100;
                //    WriteEquityLine(caption, padding, handEquity, handEquityPercent);
                //}

                //Console.WriteLine("<Tie equity>");

                //for (int hand = 0; hand < Hand.Amount; hand++)
                //{
                //    string caption = Hand.ToFormatString(hand);
                //    string padding = Hand.GetTabPadding(hand);
                //    double handEquity = player.TieEquityAmounts[hand];
                //    double handEquityPercent = handEquity / totalEquity * 100;
                //    WriteEquityLine(caption, padding, handEquity, handEquityPercent);
                //}

                //Console.WriteLine("<Negative equity>");

                //for (int hand = 0; hand < Hand.Amount; hand++)
                //{
                //    string caption = Hand.ToFormatString(hand);
                //    string padding = Hand.GetTabPadding(hand);
                //    double handEquity = player.NegativeEquityAmounts[hand];
                //    double handEquityPercent = handEquity / totalEquity * 100;
                //    WriteEquityLine(caption, padding, handEquity, handEquityPercent);
                //}

                Console.WriteLine();
            }
        }

        private static void WriteEquityLine(string caption, string padding, double equity, double equityPercent)
        {
            Console.WriteLine($"{caption}:{padding}{equity,15:0.0} {equityPercent,14:0.00000000}%");
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
    }
}
