namespace FutbolStatsWithFriends.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public int? Minute { get; set; } // ¡Excelente que sea nullable por si se les olvida el minuto!
        public bool IsPenalty { get; set; }  // true = fue penal, false = jugada normal
        public bool IsFreeKick { get; set; } //true = fue TL, false = jugada normal

        // Relación con el Partido
        public int MatchId { get; set; }
        public Match? Match { get; set; }

        // El que hizo el gol
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

       //El que asisitio
        public int? AssistedByPlayerId { get; set; }
        public Player? AssistedByPlayer { get; set; }
    }
}