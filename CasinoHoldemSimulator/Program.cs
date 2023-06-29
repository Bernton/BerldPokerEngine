using BerldPokerEngine.Poker;

namespace CasinoHoldemSimulator
{
    internal class Program
    {
        private static bool _isExtended = false;

        private static void Main(string[] args)
        {
            string[] extendedArgs = args.Where(c => c == "-e" || c == "--Extended").ToArray();
            _isExtended = extendedArgs.Any();

            args = args.Except(extendedArgs).ToArray();

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
                    Console.Error.WriteLine("Duplicate card input");
                    Environment.Exit(1);
                }

                int[] winningsByKind = RoundEngine.EvaluateRound(playerCards, flopCards);
                int winnings = winningsByKind.Sum();

                bool shouldFold = RoundEngine.FoldWinnings > winnings;
                string action = shouldFold ? "Fold" : "Continue";
                double evRatio = winnings / (double)RoundEngine.RoundIterationAmount;
                string signText = evRatio > 0 ? "win" : "loss";

                Console.WriteLine($"{action} [Average {signText} of {Math.Abs(evRatio):0.00} times the ante]");
                Console.WriteLine();

                if (_isExtended)
                {
                    for (int i = 0; i < WinningKind.Amount; i++)
                    {
                        string caption = WinningKind.ToString(i);
                        string padding = WinningKind.GetPadding(i);
                        int winningByKind = winningsByKind[i];

                        Console.WriteLine($"{caption}:{padding}{winningByKind,17}");
                    }

                    Console.WriteLine("=========================================");
                    Console.WriteLine($"Net total on continue:\t{winnings,17}");
                    Console.WriteLine($"Fold:\t\t\t{RoundEngine.FoldWinnings,17}");
                    Console.WriteLine();
                }
            }
            else
            {
                int workerAmount = 0;

                if (args.Length == 0)
                {
                    workerAmount = Math.Max(1, Environment.ProcessorCount / 4);
                }
                else if (args.Length == 1 && int.TryParse(args.First(), out int inputNumber))
                {
                    workerAmount = inputNumber;
                }

                if (workerAmount <= 0)
                {
                    Console.Error.WriteLine("Invalid argument(s)");
                    Environment.Exit(1);
                }

                Console.WriteLine($"Using {workerAmount} workers");
                Console.WriteLine("Calculating normalized rounds");

                List<NormalRound> allNormalRounds = RoundEngine.GetNormalRounds();
                ExhaustiveWorker[] workers = new ExhaustiveWorker[workerAmount];
                int lastWorkerI = workerAmount - 1;

                int normalRoundsPerWorker = allNormalRounds.Count / workerAmount;
                int normalRoundsAssigned = 0;

                for (int i = 0; i < workerAmount; i++)
                {
                    bool isLastWorker = i == lastWorkerI;

                    var skipRounds = allNormalRounds.Skip(normalRoundsAssigned);

                    var workerRounds = isLastWorker ?
                        skipRounds :
                        skipRounds.Take(normalRoundsPerWorker);

                    normalRoundsAssigned += normalRoundsPerWorker;

                    workers[i] = new()
                    {
                        NormalRounds = workerRounds.ToList()
                    };
                }

                for (int i = 0; i < workerAmount; i++)
                {
                    workers[i].Prepare();
                }

                DateTime startTime = default;

                CancellationTokenSource outputTokenSource = new();
                Task outputTask = new(async () =>
                {
                    while (!outputTokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(10_000);
                        if (outputTokenSource.IsCancellationRequested) return;
                        OutputStatus(workers, DateTime.Now - startTime, false);
                    }
                }, outputTokenSource.Token);

                Task[] workerTasks = new Task[workerAmount];

                for (int i = 0; i < workerAmount; i++)
                {
                    Task? workerTask = workers[i].Task;

                    if (workerTask is not null)
                    {
                        workerTasks[i] = workerTask;
                    }
                }

                Console.WriteLine($"Start evaluating with all {workerAmount} workers");
                Console.WriteLine();

                startTime = DateTime.Now;

                for (int i = 0; i < workerAmount; i++)
                {
                    workers[i].Start();
                }

                outputTask.Start();

                Task.WaitAll(workerTasks);
                outputTokenSource.Cancel();

                OutputStatus(workers, DateTime.Now - startTime, true);
            }
        }

        private static void OutputStatus(ExhaustiveWorker[] workers, TimeSpan elapsed, bool isFinished)
        {
            int normalRoundAmount = workers.Sum(c => c.NormalRounds.Count);
            int normalRoundsEvaluated = workers.Sum(c => c.NormalRoundsEvaluated);
            int roundsEvaluated = workers.Sum(c => c.RoundsEvaluated);
            int roundsFolded = workers.Sum(c => c.RoundsFolded);

            long totalWinnings =
                workers.Sum(c => c.ContinueWinnings.Sum()) +
                workers.Sum(c => c.FoldWinnings);

            WriteStatus(normalRoundAmount, normalRoundsEvaluated, roundsEvaluated, roundsFolded,
                totalWinnings, elapsed);

            if (isFinished)
            {
                if (_isExtended)
                {
                    for (int i = 0; i < WinningKind.Amount; i++)
                    {
                        string caption = WinningKind.ToString(i);
                        long kindWinnings = workers.Sum(c => c.ContinueWinnings[i]);

                        Console.WriteLine($"{caption}: {kindWinnings}");
                    }

                    long foldWinnings = workers.Sum(c => c.FoldWinnings);
                    Console.WriteLine($"Fold: {foldWinnings}");
                }

                Console.WriteLine($"Total winnings: {totalWinnings}");
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
                    Console.Error.WriteLine("Invalid card input");
                    Environment.Exit(1);
                }
            }

            return cards;
        }

        private static void WriteStatus(int normalRoundAmount, int normalRoundsEvaluated, int roundsEvaluated, int roundsFolded,
            long winnings, TimeSpan elapsed)
        {
            long iterationsDone = roundsEvaluated * (long)RoundEngine.RoundIterationAmount;
            double netCentWinningsPerIteration = winnings / (double)iterationsDone * 100;
            double roundsPerSecond = roundsEvaluated / elapsed.TotalSeconds;

            double progressPercent = normalRoundsEvaluated / (double)normalRoundAmount * 100;

            Console.WriteLine($"Average of {netCentWinningsPerIteration:0.000}¢ winnings per iteration");
            Console.WriteLine($"Folded {roundsFolded} of {roundsEvaluated} rounds");
            Console.WriteLine($"At {roundsPerSecond:0.00} rounds per second");
            Console.WriteLine($"Progress at {normalRoundsEvaluated}/{normalRoundAmount} ({progressPercent:0.00}%)");
            Console.WriteLine();
        }
    }
}