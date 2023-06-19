namespace BerldPokerEngine.API.Dto
{
    public class PlayerDto
    {
        public int Index { get; set; }
        public List<double> Equities { get; set; } = new();
    }
}
