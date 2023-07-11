using BerldPokerEngine.Poker;
using System.Text;
using System.Text.RegularExpressions;
using UltimateTexasHoldemSimulator;

namespace UltimateTexasHoldemMauiApp
{
    public partial class MainPage : ContentPage
    {
        private static readonly string StartText = "Start EV:\t-0.0218";

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnContentPageLoaded(object sender, EventArgs e)
        {
            entryInput.IsTextPredictionEnabled = false;
            labelOutput.Text = StartText;
        }

        private void OnCardsTextChanged(object sender, TextChangedEventArgs e)
        {
            // Remove all whitespace
            string input = Regex.Replace(e.NewTextValue, @"\s+", "");

            if (input.Length % 2 != 0)
            {
                labelOutput.Text = "Character amount not valid.";
                return;
            }

            int cardAmount = input.Length / 2;

            bool isStart = cardAmount == 0;
            bool isPreflop = cardAmount == 2;
            bool isFlop = cardAmount == 5;
            bool isRiver = cardAmount == 7;

            if (isStart)
            {
                labelOutput.Text = StartText;
                return;
            }

            if (!isPreflop && !isFlop && !isRiver)
            {
                labelOutput.Text = "Character amount not valid.";
                return;
            }

            List<Card> cards = InputToCards(input);

            if (cards is null)
            {
                labelOutput.Text = "Invalid character(s) found.";
                return;
            }

            if (cards.Distinct().Count() != cards.Count)
            {
                labelOutput.Text = "Duplicate cards found.";
                return;
            }

            var playerCards = cards.Take(2);
            var boardCards = cards.Skip(2);

            StringBuilder decisionOutput = new();
            StringBuilder evOutput = new();

            if (isPreflop)
            {
                (double raise4Value, double raise3Value, double checkValue) = Solver.EvaluatePreflopValues(playerCards);

                if (raise4Value > raise3Value && raise4Value > checkValue)
                {
                    decisionOutput.Append("Raise 4x");
                }
                else if (raise3Value >= raise4Value && raise3Value > checkValue)
                {
                    decisionOutput.Append("Raise 3x");
                }
                else
                {
                    decisionOutput.Append("Check");
                }

                evOutput.AppendLine($"Raise 4x EV:\t{raise4Value,7:0.0000}");
                evOutput.AppendLine($"Raise 3x EV:\t{raise3Value,7:0.0000}");
                evOutput.AppendLine($"Check EV:\t{checkValue,7:0.0000}");
            }
            else if (isFlop)
            {
                (double raiseValue, double checkValue) = Solver.EvaluateFlopValues(playerCards, boardCards);

                if (raiseValue > checkValue)
                {
                    decisionOutput.Append("Raise 2x");
                }
                else
                {
                    decisionOutput.Append("Check");
                }

                evOutput.AppendLine($"Raise 2x EV:\t{raiseValue,7:0.0000}");
                evOutput.AppendLine($"Check EV:\t{checkValue,7:0.0000}");
            }
            else if (isRiver)
            {
                (double raiseValue, double foldValue) = Solver.EvaluateRiverValues(playerCards, boardCards);

                if (raiseValue > foldValue)
                {
                    decisionOutput.Append("Raise 1x");
                }
                else
                {
                    decisionOutput.Append("Fold");
                }

                evOutput.AppendLine($"Raise 1x EV:\t{raiseValue,7:0.0000}");
                evOutput.AppendLine($"Fold EV:\t\t{foldValue,7:0.0000}");
            }

            labelOutput.Text = $"Action:\t\t{decisionOutput}\n\n{evOutput}";
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
                    return null;
                }
            }

            return cards;
        }
    }
}