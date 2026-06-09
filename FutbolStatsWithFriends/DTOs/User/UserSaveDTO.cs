using FutbolStatsWithFriends.Models;
using System.ComponentModel.DataAnnotations;

namespace FutbolStatsWithFriends.DTOs.User
{
    public class UserSaveDTO 
    {
        
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Roles Role { get; set; } = Roles.Player;
    }
}
