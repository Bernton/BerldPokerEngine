using BerldPokerEngine.API.Dto;
using BerldPokerEngine.Poker;
using System.Numerics;

namespace BerldPokerEngine.API
{
    public class Program
    {
        private const long MaxPermittedIterations = 300_000_000L;
        private const int RandomIterations = 10_000_000;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseFileServer();
            app.UseAuthorization();

            app.MapGet("/evaluate", (string input) =>
            {
                if (input.Length < 15)
                    return Results.BadRequest("Input must have at least length 15.");

                if (input.Length % 5 != 0)
                    return Results.BadRequest("Input length is not valid.");

                string boardInput = input[..10];
                string holeCardInput = input[10..];

                List<Card>? boardCards = InputToCards(boardInput);

                if (boardCards is null)
                    return Results.BadRequest("Invalid character(s) found in board cards.");

                List<Card> allCards = new(boardCards);

                List<string> playerCardInputs = new();
                List<List<Card>?> holeCards = new();

                int playerAmount = holeCardInput.Length / 5;

                if (playerAmount > 23)
                    return Results.BadRequest("Input defines too many players.");

                for (int i = 0; i < playerAmount; i++)
                {
                    string playerCardInput = holeCardInput.Substring(i * 5 + 1, 4);
                    List<Card>? playerCards = InputToCards(playerCardInput);

                    if (playerCards is null)
                        return Results.BadRequest("Invalid character(s) found in player cards.");

                    playerCardInputs.Add(playerCardInput);
                    holeCards.Add(playerCards);
                    allCards.AddRange(playerCards);
                }

                if (allCards.Distinct().Count() != allCards.Count)
                    return Results.BadRequest("Duplicate card input.");

                BigInteger iterationAmount = ExhaustiveEngine.CalculateIterationAmount(boardCards, holeCards);

                bool isExhaustive = iterationAmount <= MaxPermittedIterations;

                List<Player> playerStats;
                DateTime startTime = DateTime.Now;

                if (isExhaustive)
                {
                    playerStats = ExhaustiveEngine.Evaluate(boardCards, holeCards);
                }
                else
                {
                    playerStats = RandomEngine.Evaluate(boardCards, holeCards, RandomIterations);
                }

                DateTime endTime = DateTime.Now;
                TimeSpan elapsed = endTime - startTime;

                List<PlayerDto> playerDtos = playerStats.Select(ToPlayerDto).ToList();

                return Results.Ok(new EvaluationResultDto()
                {
                    IsExhaustive = isExhaustive,
                    TimeInMilliseconds = elapsed.TotalMilliseconds,
                    PlayerStats = playerDtos
                });
            });

            app.Run();
        }

        private static PlayerDto ToPlayerDto(Player player)
        {
            return new()
            {
                Index = player.Index,
                WinEquities = player.WinEquityAmounts.ToList(),
                TieEquities = player.TieEquityAmounts.ToList(),
                NegativeEquities = player.NegativeEquityAmounts.ToList()
            };
        }

        private static List<Card>? InputToCards(string input)
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