using DespachoLogistica.API.Models.DTOs;

namespace DespachoLogistica.API.Services
{
    public interface IDashboardService
    {
        Task<DashboardKPIsDTO> GetKPIsAsync();
        Task<List<RotacionProductoDTO>> GetRotacionAsync(int mes, int anio);
        Task<List<SolicitudesPorEstadoDTO>> GetSolicitudesPorEstadoAsync(int mes, int anio);
        Task<List<KardexDTO>> GetKardexAsync(int productoId, int? bodegaId, DateTime fechaDesde, DateTime fechaHasta);
    }
}