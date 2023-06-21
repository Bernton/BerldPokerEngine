namespace ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new[] { "XxXxXxXxXx XxXx" };
            }

            ConsoleHandler.Evaluate(args);
        }
    }
}