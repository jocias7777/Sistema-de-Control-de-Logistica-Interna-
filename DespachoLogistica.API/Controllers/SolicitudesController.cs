using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/solicitudes")]
    [Authorize]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _service;

        public SolicitudesController(ISolicitudService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSolicitudes(
            [FromQuery] string? estado = null,
            [FromQuery] int? solicitanteId = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var data = await _service.GetSolicitudesAsync(estado, solicitanteId, fechaDesde, fechaHasta);
                return Ok(ApiResponse<List<SolicitudResumenDTO>>.Ok(data));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudRequest request)
        {
            try
            {
                if (request.BodegaID == 0 || request.Items.Count == 0)
                    return BadRequest(ApiResponse<object>.Fail("Bodega e items son requeridos."));

                var usuarioId = int.Parse(User.FindFirstValue("UsuarioID")!);
                var email = User.FindFirstValue(ClaimTypes.Email) ?? "sistema";

                var resultado = await _service.CrearSolicitudAsync(request, usuarioId, email);
                return StatusCode(201, ApiResponse<CrearSolicitudResponse>.Ok(resultado, "Solicitud creada exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}/estado")]
        [Authorize(Roles = "Admin,Bodeguero")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NuevoEstado))
                    return BadRequest(ApiResponse<object>.Fail("El nuevo estado es requerido."));

                var usuarioId = int.Parse(User.FindFirstValue("UsuarioID")!);
                await _service.CambiarEstadoAsync(id, request.NuevoEstado, usuarioId, request.Observaciones);
                return Ok(ApiResponse<object>.Ok(null!, "Estado actualizado exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPut("{id}/despachar")]
        [Authorize(Roles = "Admin,Bodeguero")]
        public async Task<IActionResult> Despachar(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirstValue("UsuarioID")!);
                await _service.DespacharAsync(id, usuarioId);
                return Ok(ApiResponse<object>.Ok(null!, "Solicitud despachada exitosamente."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}
