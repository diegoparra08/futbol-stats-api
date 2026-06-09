namespace FutbolStatsWithFriends.DTOs.Player
{
    public class PlayerReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string PreferredFoot { get; set; } = string.Empty;
        public double OverallRating { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public bool IsActive { get; set; }

        public List<string> Positions { get; set; } = new List<string>();//Vienen en texto de una vez

        //Ratings 

        public double AvgSpeed { get; set; }
        public double AvgShooting { get; set; }
        public double AvgPassing { get; set; }
        public double AvgDribbling { get; set; }
        public double AvgDefending { get; set; }
        public double AvgPhysicality { get; set; }
        public double AvgStrength { get; set; }
        public double AvgGoalkeeping { get; set; }

    }
}
