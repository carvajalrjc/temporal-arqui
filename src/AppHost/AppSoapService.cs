using Microsoft.EntityFrameworkCore;
using MiniFacturacion.Infrastructure;
using MiniFacturacion.Domain;

namespace MiniFacturacion.AppHost;

public class AppSoapService(AppDbContext db) : IAppService
{
    public ClienteDto CrearCliente(string nombres, string documento, string email)
    {
        var c = new Cliente { Nombres = nombres, Documento = documento, Email = email };
        db.Clientes.Add(c);
        db.SaveChanges();
        return new ClienteDto { Id = c.Id, Nombres = c.Nombres, Documento = c.Documento, Email = c.Email };
    }

    public ClienteDto? ObtenerCliente(int id)
    {
        var c = db.Clientes.AsNoTracking().FirstOrDefault(x => x.Id == id);
        return c is null ? null : new ClienteDto { Id = c.Id, Nombres = c.Nombres, Documento = c.Documento, Email = c.Email };
    }

    public FacturaDto CrearFactura(int clienteId, string fechaIso, decimal monto, string moneda)
    {
        var f = new Factura { ClienteId = clienteId, Fecha = DateTime.Parse(fechaIso), Monto = monto, Moneda = moneda };
        db.Facturas.Add(f);
        db.SaveChanges();
        return new FacturaDto { Id = f.Id, ClienteId = f.ClienteId, FechaIso = f.Fecha.ToString("yyyy-MM-dd"), Monto = f.Monto, Moneda = f.Moneda };
    }

    public FacturaDto? ObtenerFactura(int id)
    {
        var f = db.Facturas.AsNoTracking().FirstOrDefault(x => x.Id == id);
        return f is null ? null : new FacturaDto { Id = f.Id, ClienteId = f.ClienteId, FechaIso = f.Fecha.ToString("yyyy-MM-dd"), Monto = f.Monto, Moneda = f.Moneda };
    }

    public MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req)
        => new() { Texto = $"{req.Monto:n2} {req.Moneda} en letras (demo {req.Cultura})" };
}
