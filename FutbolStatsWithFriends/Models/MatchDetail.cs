using FutbolStatsWithFriends.Models;

public enum TeamSide
{
    TeamA,
    TeamB
}

public class MatchDetail
{
    public int Id { get; set; }
    public TeamSide Team { get; set; }

    public int MatchId { get; set; }
    public Match? Match { get; set; }

    public int PlayerId { get; set; }
    public Player? Player { get; set; }

    public int? TacticalPositionIndex {  get; set; }

    
    public int Recoveries { get; set; }
    public int Tackles { get; set; }
    public int FoulsCommitted { get; set; }
}