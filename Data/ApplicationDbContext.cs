using API_Usuarios.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Usuarios.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Clase para la configuración de la base de datos con Entity Framework.
        // Hereda de DbContext para acceso a la base de datos.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<JefeDepartamento> JefesDepartamento { get; set; }
        public DbSet<Maestros> Maestros { get; set; } // 👈 Agregado nuevo DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configuración JefeDepartamento
            modelBuilder.Entity<JefeDepartamento>()
                .HasIndex(j => j.Nombre)
                .IsUnique();

            modelBuilder.Entity<JefeDepartamento>()
                .Property(j => j.NumeroTarjeta)
                .IsRequired();

            // Configuración Maestro
            modelBuilder.Entity<Maestros>()
                .HasIndex(m => m.ClaveMaestro)
                .IsUnique();

            modelBuilder.Entity<Maestros>()
                .Property(m => m.Nombre).IsRequired();

            modelBuilder.Entity<Maestros>()
                .Property(m => m.ApellidoPaterno).IsRequired();

            modelBuilder.Entity<Maestros>()
                .Property(m => m.CorreoElectronico).IsRequired();
        }

    }
}
