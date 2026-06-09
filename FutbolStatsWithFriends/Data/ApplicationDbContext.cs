using Microsoft.EntityFrameworkCore;
using FutbolStatsWithFriends.Models;

namespace FutbolStatsWithFriends.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //Son las 7 tablas que tenemos
        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerPosition> PlayerPositions { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchDetail> MatchDetails { get; set; }
        public DbSet<Goal> Goals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para evitar borrados en cascada múltiples (Evita conflictos en SQL Server)

            // 1. Relación en Ratings
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Player)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.PlayerId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra un jugador, se borran sus calificaciones

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Si se borra un usuario, NO borra en cascada para evitar ciclos

            // 2. Relación en MatchDetails (Muchos a Muchos)
            modelBuilder.Entity<MatchDetail>()
                .HasOne(md => md.Match)
                .WithMany(m => m.MatchDetails)
                .HasForeignKey(md => md.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchDetail>()
                .HasOne(md => md.Player)
                .WithMany(p => p.MatchDetails)
                .HasForeignKey(md => md.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Relación en Goals
            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Match)
                .WithMany(m => m.Goals)
                .HasForeignKey(g => g.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Player)
                .WithMany(p => p.Goals)
                .HasForeignKey(g => g.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Goal>()
                .HasOne(g => g.AssistedByPlayer)
                .WithMany()
                .HasForeignKey(g => g.AssistedByPlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}