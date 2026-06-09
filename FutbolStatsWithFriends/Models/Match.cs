namespace FutbolStatsWithFriends.Models
{

    public enum MatchStatus
    {
        Scheduled, 
        InPlay,    
        Finished,   
        Canceled    
    }
    public class Match
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public string Location { get; set; } = string.Empty; 
        public int TeamAScore { get; set; }
        public int TeamBScore { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        // Navigation properties
        public ICollection<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    }
}