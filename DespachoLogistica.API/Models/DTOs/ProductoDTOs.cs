namespace DespachoLogistica.API.Models.DTOs
{
    public class ProductoConStockDTO
    {
        public int ProductoID { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal StockMinimo { get; set; }
        public decimal StockActual { get; set; }
        public bool EsBajo { get; set; }
    }

    public class UpsertProductoRequest
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal StockMinimo { get; set; }
    }

    public class AjustarStockRequest
    {
        public int BodegaID { get; set; }
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class BodegaDTO
    {
        public int BodegaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
    }
}
