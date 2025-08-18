using Microsoft.EntityFrameworkCore;
using tablero_api.Models;

namespace tablero_api.Data
{
    public class AppDbContext : DbContext
    {
        

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Cuarto> Cuartos { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Partido ---
            modelBuilder.Entity<Partido>()
                .HasKey(p => p.id_Partido);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Localidad)
                .WithMany()
                .HasForeignKey(p => p.id_Localidad)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.local)
                .WithMany()
                .HasForeignKey(p => p.id_Local)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partido>()
                .HasOne(p => p.Visitante)
                .WithMany()
                .HasForeignKey(p => p.id_Visitante)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Equipo ---
            modelBuilder.Entity<Equipo>()
                .HasKey(e => e.id_Equipo);

            modelBuilder.Entity<Equipo>()
                .HasOne(e => e.Localidad)
                .WithMany()
                .HasForeignKey(e => e.id_Localidad)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Cuarto ---
            modelBuilder.Entity<Cuarto>()
                .HasKey(c => c.id_Cuarto);

            modelBuilder.Entity<Cuarto>()
                .HasOne(c => c.Partido)
                .WithMany()
                .HasForeignKey(c => c.id_Partido)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cuarto>()
                .HasOne(c => c.Equipo)
                .WithMany()
                .HasForeignKey(c => c.id_Equipo)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Localidad ---
            modelBuilder.Entity<Localidad>()
                .HasKey(l => l.id_Localidad);

            // --- Imagen ---
            modelBuilder.Entity<Imagen>()
                .HasKey(i => i.id_Imagen);
        }
    }
}
