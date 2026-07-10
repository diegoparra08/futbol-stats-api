using System.ComponentModel.DataAnnotations;

namespace FutbolStatsWithFriends.Models
{
    public class Rating
    {
        public int Id { get; set; }

        // Skill Stats (1 - 99)
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Speed { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Shooting { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Passing { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Dribbling { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Defending { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Physicality { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Strength { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 99")]
        public int Goalkeeping { get; set; }

        // Foreign Keys y Relaciones
        public int UserId { get; set; }
        public User? User { get; set; }

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}