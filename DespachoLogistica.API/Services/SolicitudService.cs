using DespachoLogistica.API.Data;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DespachoLogistica.API.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly AppDbContext _db;

        public SolicitudService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SolicitudResumenDTO>> GetSolicitudesAsync(
            string? estado, int? solicitanteId, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _db.Solicitudes
                .Include(s => s.Solicitante)
                .Include(s => s.Bodega)
                .Include(s => s.Detalles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(s => s.Estado == estado);
            if (solicitanteId.HasValue)
                query = query.Where(s => s.SolicitanteID == solicitanteId.Value);
            if (fechaDesde.HasValue)
                query = query.Where(s => s.FechaCreacion >= fechaDesde.Value);
            if (fechaHasta.HasValue)
                query = query.Where(s => s.FechaCreacion <= fechaHasta.Value);

            var lista = await query
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();

            return lista.Select(s => new SolicitudResumenDTO
            {
                SolicitudID = s.SolicitudID,
                NumeroSolicitud = s.NumeroSolicitud,
                Solicitante = s.Solicitante?.Nombre ?? "Desconocido",
                Bodega = s.Bodega?.Nombre ?? "Sin bodega",
                Estado = s.Estado,
                FechaCreacion = s.FechaCreacion,
                Observaciones = s.Observaciones,
                TotalLineas = s.Detalles.Count
            }).ToList();
        }

        public async Task<CrearSolicitudResponse> CrearSolicitudAsync(
            CrearSolicitudRequest request, int usuarioId, string email)
        {
            var count = await _db.Solicitudes.CountAsync();
            var numero = $"SOL-{DateTime.Now:yyyyMMdd}-{(count + 1):D4}";

            var solicitud = new Solicitud
            {
                NumeroSolicitud = numero,
                SolicitanteID = usuarioId,
                BodegaID = request.BodegaID,
                Estado = "Borrador",
                Observaciones = request.Observaciones,
                FechaCreacion = DateTime.Now,
                Detalles = request.Items.Select(i => new SolicitudDetalle
                {
                    ProductoID = i.ProductoID,
                    Cantidad = i.Cantidad
                }).ToList()
            };

            _db.Solicitudes.Add(solicitud);
            await _db.SaveChangesAsync();

            return new CrearSolicitudResponse
            {
                SolicitudID = solicitud.SolicitudID,
                NumeroSolicitud = solicitud.NumeroSolicitud,
                Estado = solicitud.Estado
            };
        }

        public async Task CambiarEstadoAsync(
            int solicitudId, string nuevoEstado, int usuarioId, string? observaciones)
        {
            var solicitud = await _db.Solicitudes.FindAsync(solicitudId)
                ?? throw new Exception("Solicitud no encontrada.");

            var estadosValidos = new[] { "Borrador", "Pendiente", "Aprobada", "Rechazada", "Despachada" };
            if (!estadosValidos.Contains(nuevoEstado))
                throw new Exception($"Estado '{nuevoEstado}' no es válido.");

            solicitud.Estado = nuevoEstado;
            if (!string.IsNullOrWhiteSpace(observaciones))
                solicitud.Observaciones = observaciones;

            await _db.SaveChangesAsync();
        }

        public async Task DespacharAsync(int solicitudId, int usuarioId)
        {
            var solicitud = await _db.Solicitudes
                .Include(s => s.Detalles)
                .FirstOrDefaultAsync(s => s.SolicitudID == solicitudId)
                ?? throw new Exception("Solicitud no encontrada.");

            if (solicitud.Estado != "Aprobada")
                throw new Exception("Solo se pueden despachar solicitudes aprobadas.");

            foreach (var detalle in solicitud.Detalles)
            {
                var stock = await _db.Stocks
                    .FirstOrDefaultAsync(s => s.ProductoID == detalle.ProductoID
                                           && s.BodegaID == solicitud.BodegaID);

                if (stock == null)
                    throw new Exception($"No hay stock registrado para el producto ID {detalle.ProductoID} en esta bodega.");

                if (stock.Cantidad < detalle.Cantidad)
                    throw new Exception($"Stock insuficiente para producto ID {detalle.ProductoID}. Disponible: {stock.Cantidad}.");

                var stockAntes = stock.Cantidad;
                stock.Cantidad -= detalle.Cantidad;

                // ✅ Kardex con trazabilidad completa
                _db.KardexMovimientos.Add(new KardexMovimiento
                {
                    ProductoID = detalle.ProductoID,
                    BodegaID = solicitud.BodegaID,
                    Tipo = "Salida",
                    Cantidad = detalle.Cantidad,
                    StockAntes = stockAntes,
                    StockDespues = stock.Cantidad,
                    Fecha = DateTime.Now,
                    UsuarioID = usuarioId,
                    Referencia = solicitud.NumeroSolicitud,
                    Observacion = $"Despacho solicitud {solicitud.NumeroSolicitud}"
                });
            }

            solicitud.Estado = "Despachada";
            await _db.SaveChangesAsync();
        }
    }
}
