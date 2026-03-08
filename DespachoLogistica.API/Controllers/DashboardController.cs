using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetKPIs()
        {
            var data = await _service.GetKPIsAsync();
            return Ok(ApiResponse<DashboardKPIsDTO>.Ok(data));
        }

        [HttpGet("rotacion")]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> GetRotacion(
            [FromQuery] int mes = 0,
            [FromQuery] int anio = 0)
        {
            if (mes == 0) mes = DateTime.Now.Month;
            if (anio == 0) anio = DateTime.Now.Year;

            var data = await _service.GetRotacionAsync(mes, anio);
            return Ok(ApiResponse<List<RotacionProductoDTO>>.Ok(data));
        }

        [HttpGet("solicitudes-por-estado")]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> GetSolicitudesPorEstado(
            [FromQuery] int mes = 0,
            [FromQuery] int anio = 0)
        {
            if (mes == 0) mes = DateTime.Now.Month;
            if (anio == 0) anio = DateTime.Now.Year;

            var data = await _service.GetSolicitudesPorEstadoAsync(mes, anio);
            return Ok(ApiResponse<List<SolicitudesPorEstadoDTO>>.Ok(data));
        }

        [HttpGet("kardex")]
        public async Task<IActionResult> GetKardex(
            [FromQuery] int productoId,
            [FromQuery] int? bodegaId = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            if (productoId == 0)
                return BadRequest(ApiResponse<object>.Fail("ProductoID es requerido."));

            var desde = fechaDesde ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var hasta = fechaHasta ?? DateTime.Now;

            var data = await _service.GetKardexAsync(productoId, bodegaId, desde, hasta);
            return Ok(ApiResponse<List<KardexDTO>>.Ok(data));
        }

        [HttpGet("exportar-kardex")]
        public async Task<IActionResult> ExportarKardex(
            [FromQuery] int productoId,
            [FromQuery] int? bodegaId = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            if (productoId == 0)
                return BadRequest(ApiResponse<object>.Fail("ProductoID es requerido."));

            var desde = fechaDesde ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var hasta = fechaHasta ?? DateTime.Now;

            var data = await _service.GetKardexAsync(productoId, bodegaId, desde, hasta);

            var csv = new StringBuilder();
            csv.AppendLine("MovimientoID,Fecha,Tipo,Cantidad,StockAnterior,StockNuevo,Referencia,Usuario,Bodega");

            foreach (var k in data)
                csv.AppendLine($"{k.MovimientoID},{k.FechaMovimiento:yyyy-MM-dd HH:mm},{k.TipoMovimiento},{k.Cantidad},{k.StockAnterior},{k.StockNuevo},{k.DocumentoReferencia},{k.Usuario},{k.Bodega}");

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", $"kardex-{productoId}-{desde:yyyyMMdd}-{hasta:yyyyMMdd}.csv");
        }
    }
}