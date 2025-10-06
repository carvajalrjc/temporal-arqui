// src/Contracts/SoapContracts.cs
using System.Runtime.Serialization;
using CoreWCF;
using SSM = System.ServiceModel; // Alias para WCF clásico

namespace MiniFacturacion.Contracts
{
    // =========================
    // DTOs
    // =========================
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
        [DataMember(Order = 3)] public string FechaIso { get; set; } = ""; // yyyy-MM-dd
        [DataMember(Order = 4)] public decimal Monto { get; set; }
        [DataMember(Order = 5)] public string Moneda { get; set; } = "COP";
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class PageRequest
    {
        [DataMember(Order = 1)] public int Pagina { get; set; } = 1;
        [DataMember(Order = 2)] public int Tam { get; set; } = 20;
        [DataMember(Order = 3)] public string? Q { get; set; }
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class PageResponse<T>
    {
        [DataMember(Order = 1)] public T[] Items { get; set; } = System.Array.Empty<T>();
        [DataMember(Order = 2)] public int Total { get; set; }
    }

    [DataContract(Namespace = "urn:mini-facturacion:v1")]
    public class FacturasFiltro
    {
        [DataMember(Order = 1)] public int? ClienteId { get; set; }
        [DataMember(Order = 2)] public string? DesdeIso { get; set; }
        [DataMember(Order = 3)] public string? HastaIso { get; set; }
        [DataMember(Order = 4)] public int Pagina { get; set; } = 1;
        [DataMember(Order = 5)] public int Tam { get; set; } = 20;
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

    // =========================
    // Contrato (¡doble atributo!)
    // =========================
    // CoreWCF (server) + WCF clásico (cliente gateway)
    [ServiceContract(Namespace = "urn:mini-facturacion:v1")]
    [SSM.ServiceContract(Namespace = "urn:mini-facturacion:v1")]
    public interface IAppService
    {
        // ---- Clientes ----
        [OperationContract] [SSM.OperationContract]
        ClienteDto CrearCliente(ClienteDto nuevo);

        [OperationContract] [SSM.OperationContract]
        ClienteDto? ObtenerCliente(int id);

        [OperationContract] [SSM.OperationContract]
        ClienteDto ActualizarCliente(ClienteDto dto);

        [OperationContract] [SSM.OperationContract]
        bool EliminarCliente(int id);

        [OperationContract] [SSM.OperationContract]
        PageResponse<ClienteDto> ListarClientes(PageRequest req);

        // ---- Facturas ----
        [OperationContract] [SSM.OperationContract]
        FacturaDto CrearFactura(FacturaDto nueva);

        [OperationContract] [SSM.OperationContract]
        FacturaDto? ObtenerFactura(int id);

        [OperationContract] [SSM.OperationContract]
        FacturaDto ActualizarFactura(FacturaDto dto);

        [OperationContract] [SSM.OperationContract]
        bool EliminarFactura(int id);

        [OperationContract] [SSM.OperationContract]
        PageResponse<FacturaDto> ListarFacturas(FacturasFiltro filtro);

        // ---- Utilitario ----
        [OperationContract] [SSM.OperationContract]
        MontoALetrasResponse MontoEnLetras(MontoALetrasRequest req);
    }
}
