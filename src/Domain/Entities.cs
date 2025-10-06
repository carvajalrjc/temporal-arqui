namespace MiniFacturacion.Domain;

public class Cliente
{
    public int Id { get; set; }
    public string Nombres { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Email { get; set; } = "";
    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}

public class Factura
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "COP";
}

