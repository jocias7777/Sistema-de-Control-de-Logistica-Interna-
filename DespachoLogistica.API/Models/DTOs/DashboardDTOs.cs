namespace DespachoLogistica.API.Models.DTOs
{
    public class DashboardKPIsDTO
    {
        public int TotalProductosBajoMinimo { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int DespachosDia { get; set; }
        public int TotalMovimientosMes { get; set; }
        public int TotalProductosActivos { get; set; }
    }

    public class RotacionProductoDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal TotalSalidas { get; set; }
        public decimal PorcentajeDelTotal { get; set; }
    }

    public class SolicitudesPorEstadoDTO
    {
        public string Estado { get; set; } = string.Empty;
        public int SemanaDelMes { get; set; }
        public int Total { get; set; }
    }

    public class KardexDTO
    {
        public int MovimientoID { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal StockAnterior { get; set; }
        public decimal StockNuevo { get; set; }
        public string DocumentoReferencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Bodega { get; set; } = string.Empty;
    }
}