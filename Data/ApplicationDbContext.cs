using Microsoft.EntityFrameworkCore;
using JuegoBatallasMagicas.API.Models;

namespace JuegoBatallasMagicas.API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // DbSets para tus entidades
        public DbSet<Personaje> Personajes { get; set; } = null!;
        public DbSet<Habilidad> Habilidades { get; set; } = null!;
        public DbSet<PersonajeHabilidad> PersonajeHabilidades { get; set; } = null!;
        public DbSet<Combate> Combates { get; set; } = null!;
        public DbSet<AccionCombate> AccionesCombate { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la clave primaria compuesta para PersonajeHabilidad
            modelBuilder.Entity<PersonajeHabilidad>()
                .HasKey(ph => new { ph.PersonajeId, ph.HabilidadId });

            // Configuración de la relación entre PersonajeHabilidad y Personaje
            modelBuilder.Entity<PersonajeHabilidad>()
                .HasOne(ph => ph.Personaje)
                .WithMany(p => p.PersonajeHabilidades)
                .HasForeignKey(ph => ph.PersonajeId);

            // Configuración de la relación entre PersonajeHabilidad y Habilidad
            modelBuilder.Entity<PersonajeHabilidad>()
                .HasOne(ph => ph.Habilidad)
                .WithMany(h => h.PersonajeHabilidades)
                .HasForeignKey(ph => ph.HabilidadId);

            // Configuración de la relación entre Combate y Personajes
            modelBuilder.Entity<Combate>()
                .HasOne(c => c.Personaje1)
                .WithMany() // Personajes no tienen una colección directa de Combates donde son Personaje1
                .HasForeignKey(c => c.Personaje1Id)
                .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada (para seguridad)

            modelBuilder.Entity<Combate>()
                .HasOne(c => c.Personaje2)
                .WithMany()
                .HasForeignKey(c => c.Personaje2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Combate>()
                .HasOne(c => c.Ganador)
                .WithMany()
                .HasForeignKey(c => c.GanadorId)
                .IsRequired(false); // Puede ser nulo

            // Configuración de la relación entre AccionCombate y Personaje/Habilidad
            modelBuilder.Entity<AccionCombate>()
                .HasOne(ac => ac.PersonajeAtacante)
                .WithMany()
                .HasForeignKey(ac => ac.PersonajeAtacanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccionCombate>()
                .HasOne(ac => ac.PersonajeObjetivo)
                .WithMany()
                .HasForeignKey(ac => ac.PersonajeObjetivoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccionCombate>()
                .HasOne(ac => ac.HabilidadUsada)
                .WithMany()
                .HasForeignKey(ac => ac.HabilidadUsadaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}