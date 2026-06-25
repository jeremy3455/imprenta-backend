using ImprentaSR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImprentaSR.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos de la aplicación.
/// Configura las entidades del dominio con Fluent API para EF Core.
/// Actualmente usa InMemoryDatabase para desarrollo.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Tabla de clientes.</summary>
    public DbSet<Cliente> Clientes => Set<Cliente>();

    /// <summary>
    /// Configura el mapeo de entidades usando Fluent API.
    /// Define restricciones, longitud de campos y owned entities (Direccion).
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Telefono).HasMaxLength(20);
            entity.OwnsOne(c => c.Direccion, addr =>
            {
                addr.Property(a => a.Calle).HasColumnName("Calle").HasMaxLength(300);
                addr.Property(a => a.Ciudad).HasColumnName("Ciudad").HasMaxLength(100);
                addr.Property(a => a.Estado).HasColumnName("Estado").HasMaxLength(100);
                addr.Property(a => a.CodigoPostal).HasColumnName("CodigoPostal").HasMaxLength(20);
            });
            entity.Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
        });
    }
}
