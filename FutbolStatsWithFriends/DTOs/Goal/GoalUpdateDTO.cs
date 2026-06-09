namespace FutbolStatsWithFriends.DTOs.Goal
{
    public class GoalUpdateDTO 
    {
        public int? Minute { get; set; }
        // Foreign Keys & Relationships
        public int PlayerId { get; set; }

        public bool? IsPenalty { get; set; } 
        public bool? IsFreeKick { get; set; }
        public int? AssistedByPlayerId { get; set; }
    }
}
