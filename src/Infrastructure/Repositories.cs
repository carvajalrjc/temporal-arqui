using Microsoft.EntityFrameworkCore;
using MiniFacturacion.Domain;

namespace MiniFacturacion.Infrastructure;

public interface IClientesRepository
{
    Task<Cliente> AddAsync(Cliente c);
    Task<Cliente?> GetAsync(int id);
}

public interface IFacturasRepository
{
    Task<Factura> AddAsync(Factura f);
    Task<Factura?> GetAsync(int id);
}

public class ClientesRepository(AppDbContext db) : IClientesRepository
{
    public async Task<Cliente> AddAsync(Cliente c) { db.Clientes.Add(c); await db.SaveChangesAsync(); return c; }
    public Task<Cliente?> GetAsync(int id) => db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}

public class FacturasRepository(AppDbContext db) : IFacturasRepository
{
    public async Task<Factura> AddAsync(Factura f) { db.Facturas.Add(f); await db.SaveChangesAsync(); return f; }
    public Task<Factura?> GetAsync(int id) => db.Facturas.Include(x => x.Cliente).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
