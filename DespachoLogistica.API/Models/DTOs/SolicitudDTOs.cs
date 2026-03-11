namespace DespachoLogistica.API.Models.DTOs
{
    public class SolicitudResumenDTO
    {
        public int SolicitudID { get; set; }
        public string NumeroSolicitud { get; set; } = string.Empty;
        public string Solicitante { get; set; } = string.Empty;
        public string Bodega { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? Observaciones { get; set; }
        public int TotalLineas { get; set; }
    }

    public class CrearSolicitudRequest
    {
        public int BodegaID { get; set; }
        public string? Observaciones { get; set; }
        public List<SolicitudItemRequest> Items { get; set; } = new();
    }

    public class SolicitudItemRequest
    {
        public int ProductoID { get; set; }
        public decimal Cantidad { get; set; }
    }

    public class CrearSolicitudResponse
    {
        public int SolicitudID { get; set; }
        public string NumeroSolicitud { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class CambiarEstadoRequest
    {
        public string NuevoEstado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}