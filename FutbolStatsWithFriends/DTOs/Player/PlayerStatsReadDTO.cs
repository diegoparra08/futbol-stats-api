namespace FutbolStatsWithFriends.DTOs.Player
{
    public class PlayerStatsReadDTO
    {
        public int PlayerId { get; set; }
        public int MatchesPlayed { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
    }
}
