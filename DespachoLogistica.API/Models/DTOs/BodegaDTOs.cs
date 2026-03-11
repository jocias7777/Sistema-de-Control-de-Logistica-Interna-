// Reemplaza el BodegaDTO existente
public class BodegaDTO
{
    public int BodegaID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public bool Activa { get; set; }
    public int TotalProductos { get; set; }
    public decimal TotalUnidades { get; set; }
}

// Agrega estos dos nuevos
public class BodegaStockItemDTO
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal StockMinimo { get; set; }
    public decimal StockActual { get; set; }
    public string EstadoStock { get; set; } = string.Empty;
}

public class UpsertBodegaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public bool Activa { get; set; } = true;
}