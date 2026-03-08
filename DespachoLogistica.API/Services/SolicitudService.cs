using DespachoLogistica.API.Data;
using DespachoLogistica.API.Models.DTOs;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Xml;

namespace DespachoLogistica.API.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly DatabaseContext _db;

        public SolicitudService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<CrearSolicitudResponse> CrearSolicitudAsync(CrearSolicitudRequest request, int usuarioId, string email)
        {
            // Convertir lista de items a XML
            var xml = new StringBuilder("<Detalles>");
            foreach (var item in request.Items)
                xml.Append($"<Item ProductoID=\"{item.ProductoID}\" Cantidad=\"{item.Cantidad}\"/>");
            xml.Append("</Detalles>");

            var parameters = new[]
            {
                new SqlParameter("@SolicitanteID",   usuarioId),
                new SqlParameter("@BodegaID",        request.BodegaID),
                new SqlParameter("@Observaciones",   request.Observaciones),
                new SqlParameter("@UsuarioCreacion", email),
                new SqlParameter("@Detalles",        xml.ToString())
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_CrearSolicitud", parameters);

            if (tabla.Rows.Count == 0)
                throw new Exception("No se pudo crear la solicitud.");

            var row = tabla.Rows[0];
            return new CrearSolicitudResponse
            {
                SolicitudID = Convert.ToInt32(row["SolicitudID"]),
                NumeroSolicitud = row["NumeroSolicitud"].ToString()!,
                Estado = row["Estado"].ToString()!
            };
        }

        public async Task<List<SolicitudResumenDTO>> GetSolicitudesAsync(string? estado, int? solicitanteId, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var parameters = new[]
            {
                new SqlParameter("@Estado",        (object?)estado       ?? DBNull.Value),
                new SqlParameter("@SolicitanteID", (object?)solicitanteId ?? DBNull.Value),
                new SqlParameter("@FechaDesde",    (object?)fechaDesde   ?? DBNull.Value),
                new SqlParameter("@FechaHasta",    (object?)fechaHasta   ?? DBNull.Value)
            };

            var tabla = await _db.ExecuteStoredProcedureAsync("sp_GetSolicitudesFiltradas", parameters);
            var resultado = new List<SolicitudResumenDTO>();

            foreach (System.Data.DataRow row in tabla.Rows)
            {
                resultado.Add(new SolicitudResumenDTO
                {
                    SolicitudID = Convert.ToInt32(row["SolicitudID"]),
                    NumeroSolicitud = row["NumeroSolicitud"].ToString()!,
                    Solicitante = row["Solicitante"].ToString()!,
                    Bodega = row["Bodega"].ToString()!,
                    Estado = row["Estado"].ToString()!,
                    FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                    Observaciones = row["Observaciones"]?.ToString() ?? "",
                    TotalLineas = Convert.ToInt32(row["TotalLineas"])
                });
            }

            return resultado;
        }

        public async Task CambiarEstadoAsync(int solicitudId, string nuevoEstado, int usuarioId, string observaciones)
        {
            var parameters = new[]
            {
                new SqlParameter("@SolicitudID",   solicitudId),
                new SqlParameter("@NuevoEstado",   nuevoEstado),
                new SqlParameter("@UsuarioID",     usuarioId),
                new SqlParameter("@Observaciones", observaciones)
            };

            await _db.ExecuteNonQueryAsync("sp_CambiarEstadoSolicitud", parameters);
        }

        public async Task DespacharAsync(int solicitudId, int usuarioId)
        {
            var parameters = new[]
            {
                new SqlParameter("@SolicitudID", solicitudId),
                new SqlParameter("@UsuarioID",   usuarioId)
            };

            await _db.ExecuteNonQueryAsync("sp_DespacharSolicitud", parameters);
        }
    }
}