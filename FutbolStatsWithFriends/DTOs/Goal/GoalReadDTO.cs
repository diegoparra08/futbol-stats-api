namespace FutbolStatsWithFriends.DTOs.Goal
{
    public class GoalReadDTO
    {
        public int Id { get; set; }
        public int? Minute { get; set; }

        // Foreign Keys & Relationships
        public int MatchId { get; set; }

        public int PlayerId { get; set; }
        public string PlayerName { get; set; }

        public bool IsPenalty { get; set; }
        public bool IsFreeKick { get; set; }
        public int? AssistedByPlayerId { get; set; }
        public string AssistedByPlayerName { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
