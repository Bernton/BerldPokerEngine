namespace CasinoHoldemSimulator
{
    internal class ExhaustiveWorker
    {
        internal List<NormalRound> NormalRounds { get; set; } = new();

        internal long[] ContinueWinnings { get; private set; } = new long[WinningKind.Amount];

        internal long FoldWinnings { get; private set; }
        internal int RoundsFolded { get; private set; }

        internal int NormalRoundsEvaluated { get; private set; }
        internal int RoundsEvaluated { get; private set; }

        internal Task? Task { get; private set; }

        private CancellationTokenSource? _cancellationTokenSource;

        internal void Prepare()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            NormalRoundsEvaluated = 0;
            RoundsEvaluated = 0;
            ContinueWinnings = new long[WinningKind.Amount];
            FoldWinnings = 0;
            RoundsFolded = 0;

            Task = new Task(EvaluateRounds, _cancellationTokenSource.Token);
        }

        internal void Start()
        {
            Task?.Start();
        }

        private void EvaluateRounds()
        {
            for (int i = 0; i < NormalRounds.Count; i++)
            {
                if (_cancellationTokenSource is null ||
                    _cancellationTokenSource.IsCancellationRequested)
                    return;

                NormalRound round = NormalRounds[i];

                int[] winningsByKind = RoundEngine.EvaluateRound(round.PlayerCards, round.FlopCards);
                int winnings = winningsByKind.Sum();

                bool shouldFold = RoundEngine.FoldWinnings > winnings;

                if (shouldFold)
                {
                    RoundsFolded += round.Frequency;
                    FoldWinnings -= RoundEngine.FoldWinnings * round.Frequency;
                }
                else
                {
                    for (int kindI = 0; kindI < WinningKind.Amount; kindI++)
                    {
                        ContinueWinnings[kindI] += winningsByKind[kindI] * round.Frequency;
                    }
                }

                RoundsEvaluated += round.Frequency;
                NormalRoundsEvaluated++;
            }
        }
    }
}
