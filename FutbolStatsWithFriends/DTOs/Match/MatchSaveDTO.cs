using System;
using System.Collections.Generic;

namespace FutbolStatsWithFriends.DTOs.Match
{
   
    public class MatchSaveDTO
    {
        public DateTime MatchDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public List<MatchDetailCreateDto> MatchDetails { get; set; } = new List<MatchDetailCreateDto>();
    }

    public class MatchUpdateDTO
    {
        public DateTime MatchDate { get; set; }
        public string Location { get; set; } = string.Empty;
    }

    public class MatchDetailCreateDto
    {
        public int PlayerId { get; set; }
        public TeamSide Team { get; set; }
        public int? TacticalPositionIndex { get; set; }
    }

    public class MatchStatsUpdateDTO
    {
        public List<MatchDetailUpdateDto> PlayersStats { get; set; } = new List<MatchDetailUpdateDto>();
    }

    public class MatchDetailUpdateDto
    {
        public int PlayerId { get; set; } // Obligatorio para identificar al jugador
        public TeamSide? Team { get; set; } // Nullable opcional por si no cambia de equipo
        public int? TacticalPositionIndex { get; set; }
        public int Recoveries { get; set; }
        public int Tackles { get; set; }
        public int FoulsCommitted { get; set; }
    }
}

