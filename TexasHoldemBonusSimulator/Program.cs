using TexasHoldemBonusSimulator.Engines;

namespace TexasHoldemBonusSimulator
{
    internal class Program
    {
        private static void Main()
        {
            const int Ante = 1;
            long winnings = TreeEngine.EvaluatePreflopTree(Ante);

            double averageWinnings = winnings / (double)TreeEngine.PreflopTreeIterationAmount;
            string signText = averageWinnings > 0 ? "win" : "loss";

            Console.WriteLine($"Average {signText} of {Math.Abs(averageWinnings):0.00} times the ante");
        }
    }
}