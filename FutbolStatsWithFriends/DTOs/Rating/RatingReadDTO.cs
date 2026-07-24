namespace FutbolStatsWithFriends.DTOs.Rating
{
    public class RatingReadDTO
    {
        public int Id { get; set; }
     
        public int Speed { get; set; }      
        public int Shooting { get; set; }       
        public int Passing { get; set; }     
        public int Dribbling { get; set; }        
        public int Defending { get; set; }       
        public int Physicality { get; set; }       
        public int Strength { get; set; }       
        public int Goalkeeping { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }

        public int PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public string? CreatedAt { get; set; } = null;
        public double OverallRating { get; set; }
    }
}
