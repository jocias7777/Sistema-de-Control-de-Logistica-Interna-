using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/bodegas")]
    [Authorize]
    public class BodegasController : ControllerBase
    {
        private readonly IProductoService _service;

        public BodegasController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBodegas()
        {
            try
            {
                var data = await _service.GetBodegasAsync();
                return Ok(ApiResponse<List<BodegaDTO>>.Ok(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, detalle = ex.InnerException?.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBodega([FromBody] UpsertBodegaRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nombre))
                    return BadRequest(ApiResponse<string>.Fail("El nombre es requerido"));

                var data = await _service.CreateBodegaAsync(request);
                return Ok(ApiResponse<BodegaDTO>.Ok(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, detalle = ex.InnerException?.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBodega(int id, [FromBody] UpsertBodegaRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nombre))
                    return BadRequest(ApiResponse<string>.Fail("El nombre es requerido"));

                var data = await _service.UpdateBodegaAsync(id, request);
                return Ok(ApiResponse<BodegaDTO>.Ok(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, detalle = ex.InnerException?.Message });
            }
        }

        [HttpGet("{id}/stock")]
        public async Task<IActionResult> GetStock(int id)
        {
            try
            {
                var data = await _service.GetStockPorBodegaAsync(id);
                return Ok(ApiResponse<List<BodegaStockItemDTO>>.Ok(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, detalle = ex.InnerException?.Message });
            }
        }
    }
}