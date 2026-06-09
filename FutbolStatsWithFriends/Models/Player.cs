using System.ComponentModel.DataAnnotations;

namespace FutbolStatsWithFriends.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Player name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string PreferredFoot { get; set; } = "Right"; // Right or Left
        public int Age { get; set; }
        public double Height { get; set; }
        public bool IsActive { get; set; } = true; //todos activos por defecto. 

        public ICollection<PlayerPosition> Positions { get; set; } = new List<PlayerPosition>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    }
}