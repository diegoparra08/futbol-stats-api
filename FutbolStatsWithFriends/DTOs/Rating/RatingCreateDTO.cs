using System.ComponentModel.DataAnnotations;

namespace FutbolStatsWithFriends.DTOs.Rating
{
    public class RatingCreateDTO : RatingUpdateDTO
    {
        // Las estadisticas las tomamos del Update
        public int PlayerId { get; set; }
    }
}
