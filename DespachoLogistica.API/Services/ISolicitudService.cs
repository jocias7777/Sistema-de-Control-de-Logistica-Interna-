using DespachoLogistica.API.Models.DTOs;

namespace DespachoLogistica.API.Services
{
    public interface ISolicitudService
    {
        Task<CrearSolicitudResponse> CrearSolicitudAsync(CrearSolicitudRequest request, int usuarioId, string email);
        Task<List<SolicitudResumenDTO>> GetSolicitudesAsync(string? estado, int? solicitanteId, DateTime? fechaDesde, DateTime? fechaHasta);
        Task CambiarEstadoAsync(int solicitudId, string nuevoEstado, int usuarioId, string observaciones);
        Task DespacharAsync(int solicitudId, int usuarioId);
    }
}