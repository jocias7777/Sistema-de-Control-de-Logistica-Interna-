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
            var data = await _service.GetBodegasAsync();
            return Ok(ApiResponse<List<BodegaDTO>>.Ok(data));
        }
    }
}