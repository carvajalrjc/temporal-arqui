using MiniFacturacion.Application;

namespace MiniFacturacion.Api;

public class MiniFacturacionService(
    IClientesService clientes,
    IFacturasService facturas,
    IMontosService montos) : IMiniFacturacionService
{
    public ClienteDto CrearCliente(string nombres, string documento, string email)
    {
        var c = clientes.CrearCliente(nombres, documento, email).GetAwaiter().GetResult();
        return new ClienteDto { Id = c.Id, Nombres = c.Nombres, Documento = c.Documento, Email = c.Email };
    }

    public ClienteDto? ObtenerCliente(int id)
    {
        var c = clientes.ObtenerCliente(id).GetAwaiter().GetResult();
        return c is null ? null : new ClienteDto { Id = c.Id, Nombres = c.Nombres, Documento = c.Documento, Email = c.Email };
    }

    public FacturaDto CrearFactura(int clienteId, string fechaIso, decimal monto, string moneda)
    {
        var f = facturas.CrearFactura(clienteId, DateTime.Parse(fechaIso), monto, moneda).GetAwaiter().GetResult();
        return new FacturaDto
        {
            Id = f.Id,
            ClienteId = f.ClienteId,
            FechaIso = f.Fecha.ToString("yyyy-MM-dd"),
            Monto = f.Monto,
            Moneda = f.Moneda
        };
    }

    public FacturaDto? ObtenerFactura(int id)
    {
        var f = facturas.ObtenerFactura(id).GetAwaiter().GetResult();
        return f is null ? null : new FacturaDto
        {
            Id = f.Id,
            ClienteId = f.ClienteId,
            FechaIso = f.Fecha.ToString("yyyy-MM-dd"),
            Monto = f.Monto,
            Moneda = f.Moneda
        };
    }

    public MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req)
        => new() { Texto = montos.MontoEnLetras(req.Monto, req.Moneda, req.Cultura) };
}
