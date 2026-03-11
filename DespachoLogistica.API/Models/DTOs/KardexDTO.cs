public class KardexDTO
{
    public int MovimientoID { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public string TipoMovimiento { get; set; } = "";
    public string Producto { get; set; } = "";   // ← AGREGA ESTA
    public string Bodega { get; set; } = "";
    public decimal Cantidad { get; set; }
    public decimal StockAnterior { get; set; }
    public decimal StockNuevo { get; set; }
    public string DocumentoReferencia { get; set; } = "";
    public string Usuario { get; set; } = "";
}
