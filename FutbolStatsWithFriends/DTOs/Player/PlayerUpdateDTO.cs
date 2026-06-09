using static FutbolStatsWithFriends.Models.PlayerPosition;


namespace FutbolStatsWithFriends.DTOs.Player   
{
    public class PlayerUpdateDTO
    {
        public string? Name { get; set; } = string.Empty;
        public string? Nickname { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? PreferredFoot { get; set; } = "Right"; // Right or Left

        // Lista actualizada de posiciones (Ej: si antes era ["CB"] y ahora es ["CB", "LB"])
        public List<Positions> Positions { get; set; } = new List<Positions>();
    }
}
