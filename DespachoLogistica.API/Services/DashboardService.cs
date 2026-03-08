using DespachoLogistica.API.Data;
using DespachoLogistica.API.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace DespachoLogistica.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly DatabaseContext _db;

        public DashboardService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<DashboardKPIsDTO> GetKPIsAsync()
        {
            var tabla = await _db.ExecuteStoredProcedureAsync("sp_DashboardKPIs");

            if (tabla.Rows.Count == 0)
                return new DashboardKPIsDTO();

            var row = tabla.Rows[0];
            return new DashboardKPIsDTO
            {
                TotalProductosBajoMinimo = Convert.ToInt32(row["TotalProductosBajoMinimo"]),
                SolicitudesPendientes = Convert.ToInt32(row["SolicitudesPendientes"]),
                DespachosDia = Convert.ToInt32(row["DespachosDia"]),
                TotalMovimientosMes = Convert.ToInt32(row["TotalMovimientosMes"]),
                TotalProductosActivos = Convert.ToInt32(row["TotalProductosActivos"])
            };
        }

        public async Task<List<RotacionProductoDTO>> GetRotacionAsync(int mes, int anio)
        {
            var parameters = new[]
            {
                new SqlParameter("@Mes",  mes),
                new SqlParameter("@Anio", anio)
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_ReporteRotacionProductos", parameters);
            var resultado = new List<RotacionProductoDTO>();

            foreach (System.Data.DataRow row in tabla.Rows)
            {
                resultado.Add(new RotacionProductoDTO
                {
                    Codigo = row["Codigo"].ToString()!,
                    Nombre = row["Nombre"].ToString()!,
                    TotalSalidas = Convert.ToDecimal(row["TotalSalidas"]),
                    PorcentajeDelTotal = Convert.ToDecimal(row["PorcentajeDelTotal"])
                });
            }

            return resultado;
        }

        public async Task<List<SolicitudesPorEstadoDTO>> GetSolicitudesPorEstadoAsync(int mes, int anio)
        {
            var parameters = new[]
            {
                new SqlParameter("@Mes",  mes),
                new SqlParameter("@Anio", anio)
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_ReporteSolicitudesPorEstado", parameters);
            var resultado = new List<SolicitudesPorEstadoDTO>();

            foreach (System.Data.DataRow row in tabla.Rows)
            {
                resultado.Add(new SolicitudesPorEstadoDTO
                {
                    Estado = row["Estado"].ToString()!,
                    SemanaDelMes = Convert.ToInt32(row["SemanaDelMes"]),
                    Total = Convert.ToInt32(row["Total"])
                });
            }

            return resultado;
        }

        public async Task<List<KardexDTO>> GetKardexAsync(int productoId, int? bodegaId, DateTime fechaDesde, DateTime fechaHasta)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductoID", productoId),
                new SqlParameter("@BodegaID",   (object?)bodegaId ?? DBNull.Value),
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date)
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_GetKardexProducto", parameters);
            var resultado = new List<KardexDTO>();

            foreach (System.Data.DataRow row in tabla.Rows)
            {
                resultado.Add(new KardexDTO
                {
                    MovimientoID = Convert.ToInt32(row["MovimientoID"]),
                    FechaMovimiento = Convert.ToDateTime(row["FechaMovimiento"]),
                    TipoMovimiento = row["TipoMovimiento"].ToString()!,
                    Cantidad = Convert.ToDecimal(row["Cantidad"]),
                    StockAnterior = Convert.ToDecimal(row["StockAnterior"]),
                    StockNuevo = Convert.ToDecimal(row["StockNuevo"]),
                    DocumentoReferencia = row["DocumentoReferencia"]?.ToString() ?? "",
                    Observaciones = row["Observaciones"]?.ToString() ?? "",
                    Usuario = row["Usuario"].ToString()!,
                    Bodega = row["Bodega"].ToString()!
                });
            }

            return resultado;
        }
    }
}