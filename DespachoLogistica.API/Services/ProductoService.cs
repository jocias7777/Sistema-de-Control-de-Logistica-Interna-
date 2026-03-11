using DespachoLogistica.API.Data;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DespachoLogistica.API.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _db;

        public ProductoService(AppDbContext db)
        {
            _db = db;
        }

        // ─── PRODUCTOS ────────────────────────────────────────────────────────────

        public async Task<List<ProductoConStockDTO>> GetProductosAsync(int? bodegaId, bool soloStockBajo)
        {
            var productos = await _db.Productos.ToListAsync();

            var stocksQuery = _db.Stocks.AsQueryable();
            if (bodegaId.HasValue)
                stocksQuery = stocksQuery.Where(s => s.BodegaID == bodegaId.Value);

            var stocks = await stocksQuery.ToListAsync();

            var resultado = productos.Select(p => new ProductoConStockDTO
            {
                ProductoID = p.ProductoID,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                UnidadMedida = p.UnidadMedida,
                StockMinimo = p.StockMinimo,
                StockActual = stocks
                    .Where(s => s.ProductoID == p.ProductoID)
                    .Sum(s => s.Cantidad),
                EsBajo = stocks
                    .Where(s => s.ProductoID == p.ProductoID)
                    .Sum(s => s.Cantidad) < p.StockMinimo
            }).ToList();

            if (soloStockBajo)
                resultado = resultado.Where(p => p.EsBajo).ToList();

            return resultado;
        }

        public async Task<int> UpsertProductoAsync(UpsertProductoRequest request, int productoId, string usuarioCreacion)
        {
            if (productoId == 0)
            {
                var producto = new Producto
                {
                    Codigo = request.Codigo,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    UnidadMedida = request.UnidadMedida,
                    StockMinimo = request.StockMinimo
                };
                _db.Productos.Add(producto);
                await _db.SaveChangesAsync();
                return producto.ProductoID;
            }
            else
            {
                var producto = await _db.Productos.FindAsync(productoId);
                if (producto == null) return 0;

                producto.Codigo = request.Codigo;
                producto.Nombre = request.Nombre;
                producto.Descripcion = request.Descripcion;
                producto.UnidadMedida = request.UnidadMedida;
                producto.StockMinimo = request.StockMinimo;
                await _db.SaveChangesAsync();
                return producto.ProductoID;
            }
        }

        public async Task AjustarStockAsync(int productoId, AjustarStockRequest request, int usuarioId)
        {
            var stock = await _db.Stocks
                .FirstOrDefaultAsync(s => s.ProductoID == productoId && s.BodegaID == request.BodegaID);

            if (stock == null)
            {
                stock = new Stock
                {
                    ProductoID = productoId,
                    BodegaID = request.BodegaID,
                    Cantidad = request.Cantidad
                };
                _db.Stocks.Add(stock);
            }
            else
            {
                stock.Cantidad += request.Cantidad;
            }

            await _db.SaveChangesAsync();
        }

        // ─── BODEGAS ─────────────────────────────────────────────────────────────

        public async Task<List<BodegaDTO>> GetBodegasAsync()
        {
            var bodegas = await _db.Bodegas
                .Include(b => b.Stocks)
                .ToListAsync();

            return bodegas.Select(b => new BodegaDTO
            {
                BodegaID = b.BodegaID,
                Nombre = b.Nombre,
                Ubicacion = b.Ubicacion,
                Activa = b.Activa,
                TotalProductos = b.Stocks.Select(s => s.ProductoID).Distinct().Count(),
                TotalUnidades = b.Stocks.Any() ? b.Stocks.Sum(s => s.Cantidad) : 0
            }).ToList();
        }

        public async Task<BodegaDTO> CreateBodegaAsync(UpsertBodegaRequest request)
        {
            var bodega = new Bodega
            {
                Nombre = request.Nombre,
                Ubicacion = request.Ubicacion,
                Activa = request.Activa
            };
            _db.Bodegas.Add(bodega);
            await _db.SaveChangesAsync();

            return new BodegaDTO
            {
                BodegaID = bodega.BodegaID,
                Nombre = bodega.Nombre,
                Ubicacion = bodega.Ubicacion,
                Activa = bodega.Activa,
                TotalProductos = 0,
                TotalUnidades = 0
            };
        }

        public async Task<BodegaDTO> UpdateBodegaAsync(int id, UpsertBodegaRequest request)
        {
            var bodega = await _db.Bodegas.FindAsync(id)
                ?? throw new Exception("Bodega no encontrada");

            bodega.Nombre = request.Nombre;
            bodega.Ubicacion = request.Ubicacion;
            bodega.Activa = request.Activa;
            await _db.SaveChangesAsync();

            return new BodegaDTO
            {
                BodegaID = bodega.BodegaID,
                Nombre = bodega.Nombre,
                Ubicacion = bodega.Ubicacion,
                Activa = bodega.Activa,
                TotalProductos = await _db.Stocks
                    .Where(s => s.BodegaID == id)
                    .Select(s => s.ProductoID).Distinct().CountAsync(),
                TotalUnidades = await _db.Stocks
                    .Where(s => s.BodegaID == id)
                    .SumAsync(s => s.Cantidad)
            };
        }

        public async Task<List<BodegaStockItemDTO>> GetStockPorBodegaAsync(int bodegaId)
        {
            var stocks = await _db.Stocks
                .Where(s => s.BodegaID == bodegaId)
                .Include(s => s.Producto)
                .ToListAsync();

            return stocks.Select(s => new BodegaStockItemDTO
            {
                Codigo = s.Producto!.Codigo,
                Nombre = s.Producto.Nombre,
                UnidadMedida = s.Producto.UnidadMedida,
                StockMinimo = s.Producto.StockMinimo,
                StockActual = s.Cantidad,
                EstadoStock = s.Cantidad <= 0
                    ? "Sin Stock"
                    : s.Cantidad < s.Producto.StockMinimo
                        ? "Bajo"
                        : "OK"
            }).ToList();
        }
    }
}
