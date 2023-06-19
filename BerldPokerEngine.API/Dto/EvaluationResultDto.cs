namespace BerldPokerEngine.API.Dto
{
    public class EvaluationResultDto
    {
        public double TimeInMilliseconds { get; set; }
        public List<PlayerDto> PlayerStats { get; set; } = new();
    }
}
