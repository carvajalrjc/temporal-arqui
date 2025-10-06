using Microsoft.EntityFrameworkCore;
using MiniFacturacion.Domain;

namespace MiniFacturacion.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Factura> Facturas => Set<Factura>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Cliente>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombres).IsRequired().HasMaxLength(150);
            e.Property(x => x.Documento).IsRequired().HasMaxLength(30);
            e.HasIndex(x => x.Documento).IsUnique();
        });

        b.Entity<Factura>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Moneda).HasMaxLength(10);
            e.HasIndex(x => new { x.ClienteId, x.Fecha });
            e.HasOne(x => x.Cliente).WithMany(c => c.Facturas).HasForeignKey(x => x.ClienteId);
        });
    }
}
