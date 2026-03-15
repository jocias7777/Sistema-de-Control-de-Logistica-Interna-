using DespachoLogistica.API.Data;
using DespachoLogistica.API.Helpers;
using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwt;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, JwtHelper jwt, IConfiguration config)
        {
            _db = db;
            _jwt = jwt;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse<object>.Fail("Email y contraseña son requeridos.", 400));

            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == request.Email.Trim());

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized(ApiResponse<object>.Fail("Credenciales incorrectas.", 401));

            if (!usuario.Activo)
                return Unauthorized(ApiResponse<object>.Fail("Usuario inactivo. Contacte al administrador.", 401));

            var token = _jwt.GenerateToken(usuario.UsuarioID, usuario.Email, usuario.Rol!.Nombre, usuario.Nombre);
            var expiracion = DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpiresHours"]!));

            return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
            {
                Token = token,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol!.Nombre,
                Expiracion = expiracion
            }, "Login exitoso."));
        }

        // ⚠️ ENDPOINT TEMPORAL - BORRAR DESPUÉS DE USARLO
        [HttpGet("genhash")]
        public IActionResult GenHash()
        {
            return Ok(BCrypt.Net.BCrypt.HashPassword("admin123!"));
        }
    }
}