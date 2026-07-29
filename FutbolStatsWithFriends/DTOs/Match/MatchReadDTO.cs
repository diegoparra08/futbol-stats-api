using FutbolStatsWithFriends.DTOs.Goal;

namespace FutbolStatsWithFriends.DTOs.Match
{
    public class MatchReadDTO
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int TeamAScore { get; set; }
        public int TeamBScore { get; set; }
        public string Status { get; set; }

        // Navigation properties
        public List<MatchDetailReadDto> MatchDetails { get; set; } = new List<MatchDetailReadDto>();
        public List<GoalReadDTO> Goals { get; set; } = new List<GoalReadDTO>();
    }

    public class MatchDetailReadDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int? TacticalPositionIndex { get; set; }
        public string Team { get; set; }
        public int Recoveries { get; set; }
        public int Tackles { get; set; }
        public int FoulsCommitted { get; set; }
    }
}
