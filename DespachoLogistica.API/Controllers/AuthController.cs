using DespachoLogistica.API.Data;
using DespachoLogistica.API.Helpers;
using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DespachoLogistica.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _db;
        private readonly JwtHelper _jwt;
        private readonly IConfiguration _config;

        public AuthController(DatabaseContext db, JwtHelper jwt, IConfiguration config)
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

            var parameters = new[]
            {
                new SqlParameter("@Email", request.Email.Trim())
            };

            var result = await _db.ExecuteStoredProcedureAsync("sp_Login", parameters);

            if (result.Rows.Count == 0)
                return Unauthorized(ApiResponse<object>.Fail("Credenciales incorrectas.", 401));

            var row = result.Rows[0];

            if (Convert.ToBoolean(row["Activo"]) == false)
                return Unauthorized(ApiResponse<object>.Fail("Usuario inactivo. Contacte al administrador.", 401));

            var passwordHash = row["PasswordHash"].ToString()!;
            var passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);

            if (!passwordValido)
                return Unauthorized(ApiResponse<object>.Fail("Credenciales incorrectas.", 401));

            var usuarioId = Convert.ToInt32(row["UsuarioID"]);
            var nombre = row["Nombre"].ToString()!;
            var email = row["Email"].ToString()!;
            var rol = row["RolNombre"].ToString()!;

            var token = _jwt.GenerateToken(usuarioId, email, rol, nombre);
            var expiracion = DateTime.UtcNow.AddHours(
                int.Parse(_config["Jwt:ExpiresHours"]!));

            var response = new LoginResponse
            {
                Token = token,
                Nombre = nombre,
                Email = email,
                Rol = rol,
                Expiracion = expiracion
            };

            return Ok(ApiResponse<LoginResponse>.Ok(response, "Login exitoso."));
        }

    }
}