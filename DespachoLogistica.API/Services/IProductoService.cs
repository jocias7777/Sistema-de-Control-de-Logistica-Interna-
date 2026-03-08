using DespachoLogistica.API.Models.DTOs;

namespace DespachoLogistica.API.Services
{
    public interface IProductoService
    {
        Task<List<ProductoConStockDTO>> GetProductosAsync(int? bodegaId, bool soloStockBajo);
        Task<int> UpsertProductoAsync(UpsertProductoRequest request, int productoId, string usuarioCreacion);
        Task AjustarStockAsync(int productoId, AjustarStockRequest request, int usuarioId);
        Task<List<BodegaDTO>> GetBodegasAsync();
    }
}