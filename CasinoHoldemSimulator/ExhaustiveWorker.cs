namespace CasinoHoldemSimulator
{
    internal class ExhaustiveWorker
    {
        internal List<NormalRound> NormalRounds { get; set; } = new();

        internal int NormalRoundsEvaluated { get; private set; }
        internal int RoundsEvaluated { get; private set; }
        internal int RoundsFolded { get; private set; }
        internal long Winnings { get; private set; }

        internal Task? Task { get; private set; }

        private CancellationTokenSource? _cancellationTokenSource;

        internal void Prepare()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            NormalRoundsEvaluated = 0;
            RoundsEvaluated = 0;
            RoundsFolded = 0;
            Winnings = 0;

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

                int roundWinnings = RoundEngine.EvaluateRound(round.PlayerCards, round.FlopCards);

                bool shouldFold = RoundEngine.FoldWinnings > roundWinnings;

                if (shouldFold)
                {
                    RoundsFolded += round.Frequency;
                    Winnings += RoundEngine.FoldWinnings * round.Frequency;
                }
                else
                {
                    Winnings += roundWinnings * round.Frequency;
                }

                RoundsEvaluated += round.Frequency;
                NormalRoundsEvaluated++;
            }
        }
    }
}
