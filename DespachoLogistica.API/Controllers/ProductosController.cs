using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/productos")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductosController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos(
            [FromQuery] int? bodegaId = null,
            [FromQuery] bool soloStockBajo = false)
        {
            var data = await _service.GetProductosAsync(bodegaId, soloStockBajo);
            return Ok(ApiResponse<List<ProductoConStockDTO>>.Ok(data));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearProducto([FromBody] UpsertProductoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Codigo) || string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(ApiResponse<object>.Fail("Código y Nombre son requeridos."));

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "sistema";
            var id = await _service.UpsertProductoAsync(request, 0, email);
            return StatusCode(201, ApiResponse<object>.Ok(new { ProductoID = id }, "Producto creado exitosamente."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarProducto(int id, [FromBody] UpsertProductoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Codigo) || string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest(ApiResponse<object>.Fail("Código y Nombre son requeridos."));

            var email = User.FindFirstValue(ClaimTypes.Email) ?? "sistema";
            await _service.UpsertProductoAsync(request, id, email);
            return Ok(ApiResponse<object>.Ok(null!, "Producto actualizado exitosamente."));
        }

        [HttpPost("{id}/ajustar-stock")]
        [Authorize(Roles = "Admin,Bodeguero")]
        public async Task<IActionResult> AjustarStock(int id, [FromBody] AjustarStockRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Motivo))
                return BadRequest(ApiResponse<object>.Fail("El motivo es requerido."));

            var usuarioId = int.Parse(User.FindFirstValue("UsuarioID")!);
            await _service.AjustarStockAsync(id, request, usuarioId);
            return Ok(ApiResponse<object>.Ok(null!, "Stock ajustado exitosamente."));
        }
    }
}