namespace BerldPokerEngine.API.Dto
{
    public class PlayerDto
    {
        public int Index { get; set; }
        public List<long> WinEquities { get; set; } = new();
        public List<long> NegativeEquities { get; set; } = new();
        public List<double> TieEquities { get; set; } = new();
    }
}
