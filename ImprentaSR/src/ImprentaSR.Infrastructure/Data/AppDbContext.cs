using ImprentaSR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImprentaSR.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

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
