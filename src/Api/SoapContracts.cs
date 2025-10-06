using System.Runtime.Serialization;
using CoreWCF;

namespace MiniFacturacion.Api
{
    // ===== DataContracts =====
    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class ClienteDto
    {
        [DataMember(Order = 1)] public int Id { get; set; }
        [DataMember(Order = 2)] public string Nombres { get; set; } = "";
        [DataMember(Order = 3)] public string Documento { get; set; } = "";
        [DataMember(Order = 4)] public string Email { get; set; } = "";
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class FacturaDto
    {
        [DataMember(Order = 1)] public int Id { get; set; }
        [DataMember(Order = 2)] public int ClienteId { get; set; }
        [DataMember(Order = 3)] public string FechaIso { get; set; } = "";
        [DataMember(Order = 4)] public decimal Monto { get; set; }
        [DataMember(Order = 5)] public string Moneda { get; set; } = "COP";
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class MontoALetrasRequest
    {
        [DataMember(Order = 1)] public decimal Monto { get; set; }
        [DataMember(Order = 2)] public string Moneda { get; set; } = "COP";
        [DataMember(Order = 3)] public string Cultura { get; set; } = "es-CO";
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class MontoALetrasResponse
    {
        [DataMember(Order = 1)] public string Texto { get; set; } = "";
    }

    // ===== ServiceContract =====
    [ServiceContract(Namespace = "urn:mini-facturacion:v1")]
    public interface IMiniFacturacionService
    {
        [OperationContract] ClienteDto CrearCliente(string nombres, string documento, string email);
        [OperationContract] ClienteDto? ObtenerCliente(int id);
        [OperationContract] FacturaDto CrearFactura(int clienteId, string fechaIso, decimal monto, string moneda);
        [OperationContract] FacturaDto? ObtenerFactura(int id);
        [OperationContract] MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req);
    }
}
