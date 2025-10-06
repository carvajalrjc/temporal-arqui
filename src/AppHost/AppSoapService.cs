using CoreWCF;
using Microsoft.EntityFrameworkCore;

// Contratos (IAppService y DTOs)
using MiniFacturacion.Contracts;

// Infraestructura (DbContext)
using MiniFacturacion.Infrastructure;

// Entidades de dominio (Cliente, Factura)
using DomCliente  = MiniFacturacion.Domain.Cliente;
using DomFactura  = MiniFacturacion.Domain.Factura;

namespace MiniFacturacion.AppHost;

public class AppSoapService(AppDbContext db) : IAppService
{
    // ===== CLIENTES =====
    public ClienteDto CrearCliente(ClienteDto nuevo)
    {
        var ent = new DomCliente { Nombres = nuevo.Nombres, Documento = nuevo.Documento, Email = nuevo.Email };
        db.Clientes.Add(ent);
        db.SaveChanges();
        nuevo.Id = ent.Id;
        return nuevo;
    }

    public ClienteDto? ObtenerCliente(int id) =>
        db.Clientes.AsNoTracking()
          .Where(c => c.Id == id)
          .Select(c => new ClienteDto { Id = c.Id, Nombres = c.Nombres, Documento = c.Documento, Email = c.Email })
          .FirstOrDefault();

    public ClienteDto ActualizarCliente(ClienteDto dto)
    {
        var ent = db.Clientes.Find(dto.Id) ?? throw new FaultException("Cliente no existe");
        ent.Nombres = dto.Nombres; ent.Documento = dto.Documento; ent.Email = dto.Email;
        db.SaveChanges();
        return dto;
    }

    public bool EliminarCliente(int id)
    {
        var ent = db.Clientes.Find(id);
        if (ent is null) return false;
        db.Clientes.Remove(ent);
        db.SaveChanges();
        return true;
    }

    public PageResponse<ClienteDto> ListarClientes(PageRequest req)
    {
        var q = db.Clientes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Q))
            q = q.Where(c => c.Nombres.Contains(req.Q) || c.Documento.Contains(req.Q));

        var total = q.Count();
        var items = q.OrderBy(c => c.Id)
                     .Skip((req.Pagina - 1) * req.Tam)
                     .Take(req.Tam)
                     .Select(c => new ClienteDto { Id = c.Id, Nombres = c.Nombres, Documento = c.Documento, Email = c.Email })
                     .ToArray();

        return new PageResponse<ClienteDto> { Items = items, Total = total };
    }

    // ===== FACTURAS =====
    public FacturaDto CrearFactura(FacturaDto nueva)
    {
        var ent = new DomFactura
        {
            ClienteId = nueva.ClienteId,
            Fecha     = DateTime.Parse(nueva.FechaIso),
            Monto     = nueva.Monto,
            Moneda    = nueva.Moneda
        };
        db.Facturas.Add(ent);
        db.SaveChanges();
        nueva.Id = ent.Id;
        return nueva;
    }

    public FacturaDto? ObtenerFactura(int id) =>
        db.Facturas.AsNoTracking()
          .Where(f => f.Id == id)
          .Select(f => new FacturaDto {
              Id = f.Id, ClienteId = f.ClienteId,
              FechaIso = f.Fecha.ToString("yyyy-MM-dd"),
              Monto = f.Monto, Moneda = f.Moneda
          }).FirstOrDefault();

    public FacturaDto ActualizarFactura(FacturaDto dto)
    {
        var ent = db.Facturas.Find(dto.Id) ?? throw new FaultException("Factura no existe");
        ent.ClienteId = dto.ClienteId;
        ent.Fecha     = DateTime.Parse(dto.FechaIso);
        ent.Monto     = dto.Monto;
        ent.Moneda    = dto.Moneda;
        db.SaveChanges();
        return dto;
    }

    public bool EliminarFactura(int id)
    {
        var ent = db.Facturas.Find(id);
        if (ent is null) return false;
        db.Facturas.Remove(ent);
        db.SaveChanges();
        return true;
    }

    public PageResponse<FacturaDto> ListarFacturas(FacturasFiltro filtro)
    {
        var q = db.Facturas.AsNoTracking().AsQueryable();
        if (filtro.ClienteId.HasValue) q = q.Where(f => f.ClienteId == filtro.ClienteId.Value);
        if (!string.IsNullOrWhiteSpace(filtro.DesdeIso)) q = q.Where(f => f.Fecha >= DateTime.Parse(filtro.DesdeIso));
        if (!string.IsNullOrWhiteSpace(filtro.HastaIso)) q = q.Where(f => f.Fecha <  DateTime.Parse(filtro.HastaIso).AddDays(1));

        var total = q.Count();
        var items = q.OrderByDescending(f => f.Fecha)
                     .Skip((filtro.Pagina - 1) * filtro.Tam)
                     .Take(filtro.Tam)
                     .Select(f => new FacturaDto {
                         Id = f.Id, ClienteId = f.ClienteId,
                         FechaIso = f.Fecha.ToString("yyyy-MM-dd"),
                         Monto = f.Monto, Moneda = f.Moneda
                     }).ToArray();

        return new PageResponse<FacturaDto> { Items = items, Total = total };
    }

    // ===== DEMO LETRAS =====
    public MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req)
        => new() { Texto = $"{req.Monto:0.00} {req.Moneda} en letras (demo {req.Cultura})" };
}
