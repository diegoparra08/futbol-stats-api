namespace FutbolStatsWithFriends.DTOs.Player
{
    public class PlayerCreateDTO : PlayerUpdateDTO
    {
        public int Age { get; set; }
        public double Height { get; set; }
    }
}
