using MiniFacturacion.Domain;
using MiniFacturacion.Infrastructure;

namespace MiniFacturacion.Application;

public interface IClientesService
{
    Task<Cliente> CrearCliente(string nombres, string documento, string email);
    Task<Cliente?> ObtenerCliente(int id);
}
public interface IFacturasService
{
    Task<Factura> CrearFactura(int clienteId, DateTime fecha, decimal monto, string moneda);
    Task<Factura?> ObtenerFactura(int id);
}
public interface IMontosService
{
    string MontoEnLetras(decimal monto, string moneda, string cultura = "es-CO");
}

public class ClientesService(IClientesRepository repo) : IClientesService
{
    public Task<Cliente?> ObtenerCliente(int id) => repo.GetAsync(id);
    public Task<Cliente> CrearCliente(string n, string d, string e)
        => repo.AddAsync(new Cliente { Nombres = n, Documento = d, Email = e });
}

public class FacturasService(IFacturasRepository repo) : IFacturasService
{
    public Task<Factura?> ObtenerFactura(int id) => repo.GetAsync(id);
    public Task<Factura> CrearFactura(int clienteId, DateTime fecha, decimal monto, string moneda)
        => repo.AddAsync(new Factura { ClienteId = clienteId, Fecha = fecha, Monto = monto, Moneda = moneda });
}

public class MontosService : IMontosService
{
    public string MontoEnLetras(decimal monto, string moneda, string cultura = "es-CO")
        => $"{monto:n2} {moneda} en letras (demo {cultura})";
}
