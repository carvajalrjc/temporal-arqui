using CoreWCF;
using MiniFacturacion.Contracts;
using System.ServiceModel; // WCF client

namespace MiniFacturacion.Api;

public class GatewaySoapService(IAppService sad) : IAppService
{
  // simple pass-through; aquí podrías añadir validaciones/caching/circuit-breaker
  public ClienteDto CrearCliente(ClienteDto nuevo)                => sad.CrearCliente(nuevo);
  public ClienteDto? ObtenerCliente(int id)                       => sad.ObtenerCliente(id);
  public ClienteDto ActualizarCliente(ClienteDto dto)             => sad.ActualizarCliente(dto);
  public bool EliminarCliente(int id)                             => sad.EliminarCliente(id);
  public PageResponse<ClienteDto> ListarClientes(PageRequest req) => sad.ListarClientes(req);

  public FacturaDto CrearFactura(FacturaDto nueva)                    => sad.CrearFactura(nueva);
  public FacturaDto? ObtenerFactura(int id)                           => sad.ObtenerFactura(id);
  public FacturaDto ActualizarFactura(FacturaDto dto)                 => sad.ActualizarFactura(dto);
  public bool EliminarFactura(int id)                                 => sad.EliminarFactura(id);
  public PageResponse<FacturaDto> ListarFacturas(FacturasFiltro fil)  => sad.ListarFacturas(fil);

  public MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req)  => sad.MontoEnLetras(req);
}
