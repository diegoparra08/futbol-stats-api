namespace FutbolStatsWithFriends.Models
{
    public class PlayerPosition
    {
        public enum Positions
        {
            GK,
            CB,
            RB,
            LB,
            DCM,
            CM,
            CAM,
            RM,
            LM,
            RW,
            LW,
            CF,
            ST
        }
        public int Id { get; set; }
        public Positions PositionName { get; set; } = Positions.CM; 
        //Foreign keys
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
    }
}