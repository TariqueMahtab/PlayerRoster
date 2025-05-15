namespace PlayerRoster.Server.Data.Models
{
    public class PlayerCsv
    {
        public int Rank { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Position { get; set; } = null!;
        public float Points { get; set; }
        public int Games { get; set; }
        public float Minutes { get; set; }
        public float Rebounds { get; set; }
        public float Assists { get; set; }
        public float Steal { get; set; }
        public float Block { get; set; }
    }
}
