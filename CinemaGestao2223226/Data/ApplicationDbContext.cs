using CinemaGestao2223226.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaGestao2223226.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Reserva -> Sessao relationship
            // When a session is deleted, set SessaoId to null (keep reservation for refund tracking)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Sessao)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.SessaoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}