using System.ServiceModel; // cliente WCF (paquetes del runtime de .NET)
using System.Runtime.Serialization;

namespace MiniFacturacion.Api.AppClient
{
    [ServiceContract(Namespace = "urn:mini-facturacion:v1")]
    public interface IAppService
    {
        [OperationContract] ClienteDto CrearCliente(string nombres, string documento, string email);
        [OperationContract] ClienteDto? ObtenerCliente(int id);
        [OperationContract] FacturaDto CrearFactura(int clienteId, string fechaIso, decimal monto, string moneda);
        [OperationContract] FacturaDto? ObtenerFactura(int id);
        [OperationContract] MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req);
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class ClienteDto { /* mismos campos */ }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class FacturaDto { /* mismos campos */ }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class MontoALetrasRequest { /* mismos campos */ }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class MontoALetrasResponse { [DataMember(Order = 1)] public string Texto { get; set; } = ""; }
}
