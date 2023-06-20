namespace ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new[] { "XxXxXxXxXx Jc5c XxXx" };
            }

            ConsoleHandler.Evaluate(args);
        }
    }
}