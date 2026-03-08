using DespachoLogistica.API.Data;
using DespachoLogistica.API.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace DespachoLogistica.API.Services
{
    public class ProductoService : IProductoService
    {
        private readonly DatabaseContext _db;

        public ProductoService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<ProductoConStockDTO>> GetProductosAsync(int? bodegaId, bool soloStockBajo)
        {
            var parameters = new[]
            {
                new SqlParameter("@BodegaID",      (object?)bodegaId ?? DBNull.Value),
                new SqlParameter("@SoloStockBajo", soloStockBajo ? 1 : 0)
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_GetProductosConStock", parameters);
            var resultado = new List<ProductoConStockDTO>();

            foreach (System.Data.DataRow row in tabla.Rows)
            {
                resultado.Add(new ProductoConStockDTO
                {
                    ProductoID = Convert.ToInt32(row["ProductoID"]),
                    Codigo = row["Codigo"].ToString()!,
                    Nombre = row["Nombre"].ToString()!,
                    UnidadMedida = row["UnidadMedida"].ToString()!,
                    StockMinimo = Convert.ToDecimal(row["StockMinimo"]),
                    StockActual = Convert.ToDecimal(row["StockActual"]),
                    EsBajo = Convert.ToBoolean(row["EsBajo"])
                });
            }

            return resultado;
        }

        public async Task<int> UpsertProductoAsync(UpsertProductoRequest request, int productoId, string usuarioCreacion)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductoID",      productoId == 0 ? DBNull.Value : productoId),
                new SqlParameter("@Codigo",          request.Codigo),
                new SqlParameter("@Nombre",          request.Nombre),
                new SqlParameter("@Descripcion",     request.Descripcion),
                new SqlParameter("@UnidadMedida",    request.UnidadMedida),
                new SqlParameter("@StockMinimo",     request.StockMinimo),
                new SqlParameter("@UsuarioCreacion", usuarioCreacion)
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_UpsertProducto", parameters);

            if (tabla.Rows.Count > 0)
                return Convert.ToInt32(tabla.Rows[0]["ProductoID"]);

            return 0;
        }

        public async Task AjustarStockAsync(int productoId, AjustarStockRequest request, int usuarioId)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductoID", productoId),
                new SqlParameter("@BodegaID",   request.BodegaID),
                new SqlParameter("@Cantidad",   request.Cantidad),
                new SqlParameter("@Motivo",     request.Motivo),
                new SqlParameter("@UsuarioID",  usuarioId)
            };

            await _db.ExecuteNonQueryAsync("sp_AjustarStock", parameters);
        }

        public async Task<List<BodegaDTO>> GetBodegasAsync()
        {
            var tabla = await _db.ExecuteStoredProcedureAsync("sp_GetBodegas");
            var resultado = new List<BodegaDTO>();

            foreach (System.Data.DataRow row in tabla.Rows)
            {
                resultado.Add(new BodegaDTO
                {
                    BodegaID = Convert.ToInt32(row["BodegaID"]),
                    Nombre = row["Nombre"].ToString()!,
                    Ubicacion = row["Ubicacion"].ToString()!,
                    Responsable = row["Responsable"].ToString()!
                });
            }

            return resultado;
        }
    }
}