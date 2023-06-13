using BerldPokerEngine.Poker;

namespace BerldPokerEngine
{
    internal class Program
    {
        private static int? RankFromChar(char rankChar)
        {
            return rankChar switch
            {
                '2' => Rank.Deuce,
                '3' => Rank.Tray,
                '4' => Rank.Four,
                '5' => Rank.Five,
                '6' => Rank.Six,
                '7' => Rank.Seven,
                '8' => Rank.Eight,
                '9' => Rank.Nine,
                'T' => Rank.Ten,
                'J' => Rank.Jack,
                'Q' => Rank.Queen,
                'K' => Rank.King,
                'A' => Rank.Ace,
                _ => null
            };
        }

        private static int? SuitFromChar(char suitChar)
        {
            return suitChar switch
            {
                'c' => Suit.Clubs,
                'd' => Suit.Diamonds,
                'h' => Suit.Hearts,
                's' => Suit.Spades,
                _ => null
            };
        }

        private static void Main(string[] args)
        {
            args = new[] { "XxXxXxXxXx JcTc AcKd" };

            Evaluate(args);
        }

        private static List<Card> inputToCards(string input)
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

                int? rank = RankFromChar(rankChar);
                int? suit = SuitFromChar(suitChar);

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

            List<Card> boardCards = inputToCards(boardInput);

            string holeCardInput = input[10..];

            List<string> playerCardInputs = new();
            List<List<Card>> holeCards = new();

            int playerAmount = holeCardInput.Length / 5;

            for (int i = 0; i < playerAmount; i++)
            {
                string playerCardInput = holeCardInput.Substring(i * 5 + 1, 4);
                List<Card> playerCards = inputToCards(playerCardInput);

                playerCardInputs.Add(playerCardInput);
                holeCards.Add(playerCards);
            }

            int wildBoardCardAmount = 5 - boardCards.Count;
            int wildPlayerCardAmount = 0;

            foreach (List<Card> playerCards in holeCards)
            {
                wildPlayerCardAmount = 2 - playerCards.Count;
            }

            List<double[]> equities = new();

            DateTime startTime = DateTime.Now;

            if (wildBoardCardAmount == 5 && wildPlayerCardAmount == 0)
            {
                equities = Engine.Evaluate_5_0(holeCards);
            }
            else
            {
                Console.Error.WriteLine("Format not supported.");
                Environment.Exit(1);
            }

            DateTime endTime = DateTime.Now;
            TimeSpan elapsed = endTime - startTime;

            double totalEquity = equities.Sum(c => c.Sum());
            double equityPerMillisecond = totalEquity / elapsed.TotalMilliseconds;

            Console.WriteLine($"Time: {elapsed.TotalMilliseconds:0.0} ms");
            Console.WriteLine($"Speed: {equityPerMillisecond:0} equity/ms");
            Console.WriteLine($"Total:\t\t\t{totalEquity,10:0.0} {100.0,14:0.00000000}%");
            Console.WriteLine();

            for (int i = 0; i < equities.Count; i++)
            {
                double[] playerEquity = equities[i];
                double totalPlayerEquity = playerEquity.Sum();
                double totalPlayerEquityPercent = totalPlayerEquity / totalEquity * 100;

                Console.WriteLine($"Player {(i + 1)} - {playerCardInputs[i]}");
                Console.WriteLine($"Equity:\t\t\t{totalPlayerEquity,10:0.0} {totalPlayerEquityPercent,14:0.00000000}%");

                for (int j = 0; j < 10; j++)
                {
                    int hand = j;
                    double handEquity = playerEquity[j];
                    double handEquityPercent = handEquity / totalEquity * 100;

                    Console.WriteLine($"{Hand.ToFormatString(hand)}:{Hand.GetTabPadding(hand)}{handEquity,10:0.0} {handEquityPercent,14:0.00000000}%");
                }

                Console.WriteLine();
            }
        }
    }
}