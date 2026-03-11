using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        // GET /api/usuarios
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsuarios()
        {
            var res = await _service.GetUsuariosAsync();
            return Ok(res);
        }

        // GET /api/usuarios/roles
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoles()
        {
            var res = await _service.GetRolesAsync();
            return Ok(res);
        }

        // POST /api/usuarios
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Nombre, Email y Password son requeridos");

            var res = await _service.CrearUsuarioAsync(request);
            return Ok(res);
        }

        // PUT /api/usuarios/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] EditarUsuarioRequest request)
        {
            request.UsuarioID = id;
            var res = await _service.EditarUsuarioAsync(request);
            return Ok(res);
        }

        // PUT /api/usuarios/{id}/toggle
        [HttpPut("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActivo(int id, [FromBody] ToggleActivoRequest request)
        {
            var res = await _service.ToggleActivoAsync(id, request.Activo);
            return Ok(res);
        }

        // PUT /api/usuarios/{id}/password
        [HttpPut("{id}/password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NuevoPassword))
                return BadRequest("La nueva contraseña es requerida");

            var res = await _service.CambiarPasswordAsync(id, request);
            return Ok(res);
        }
    }

    public class ToggleActivoRequest
    {
        public bool Activo { get; set; }
    }
}