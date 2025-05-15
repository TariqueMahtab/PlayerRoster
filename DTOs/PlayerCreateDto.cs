namespace PlayerRoster.Server.DTOs
{
    public class PlayerCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public double Ppg { get; set; }
        public double Rpg { get; set; }
        public double Apg { get; set; }
        public int TeamId { get; set; }
    }
}
