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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales para la entidad Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

    }
}
