namespace BerldPokerEngine.API.Dto
{
    public class EvaluationResultDto
    {
        public bool IsExhaustive { get; set; }
        public double TimeInMilliseconds { get; set; }
        public List<PlayerDto> PlayerStats { get; set; } = new();
    }
}
