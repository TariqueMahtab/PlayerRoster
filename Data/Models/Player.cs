namespace PlayerRoster.Server.Data.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public float PPG { get; set; }
        public float RPG { get; set; }
        public float APG { get; set; }
    }
}
