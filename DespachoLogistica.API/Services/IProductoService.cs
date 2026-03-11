using DespachoLogistica.API.Models.DTOs;

namespace DespachoLogistica.API.Services
{
    public interface IProductoService
    {
        Task<List<ProductoConStockDTO>> GetProductosAsync(int? bodegaId, bool soloStockBajo);
        Task<int> UpsertProductoAsync(UpsertProductoRequest request, int productoId, string usuarioCreacion);
        Task AjustarStockAsync(int productoId, AjustarStockRequest request, int usuarioId);

        // Bodegas
        Task<List<BodegaDTO>> GetBodegasAsync();
        Task<BodegaDTO> CreateBodegaAsync(UpsertBodegaRequest request);
        Task<BodegaDTO> UpdateBodegaAsync(int id, UpsertBodegaRequest request);
        Task<List<BodegaStockItemDTO>> GetStockPorBodegaAsync(int bodegaId);
    }
}