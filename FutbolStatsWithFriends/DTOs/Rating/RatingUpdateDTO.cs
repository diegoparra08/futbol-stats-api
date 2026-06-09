using System.ComponentModel.DataAnnotations;

namespace FutbolStatsWithFriends.DTOs.Rating
{
    public class RatingUpdateDTO
    {
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
    }
}
