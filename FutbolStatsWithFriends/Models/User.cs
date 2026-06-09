using System.ComponentModel.DataAnnotations;

namespace FutbolStatsWithFriends.Models
{
    public enum Roles
    {
        Admin,
        Player,
        Coach
    }
    public class User
    {  
        public int Id { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.Player;
        public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
    }
}
