using DespachoLogistica.API.Data;
using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;
using DespachoLogistica.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DespachoLogistica.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _db;

        public UsuarioService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<List<UsuarioDTO>>> GetUsuariosAsync()
        {
            var usuarios = await _db.Usuarios
                .Include(u => u.Rol)
                .OrderByDescending(u => u.FechaCreacion)
                .Select(u => new UsuarioDTO
                {
                    UsuarioID = u.UsuarioID,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Rol = u.Rol!.Nombre,
                    RolID = u.RolID,
                    Activo = u.Activo,
                    FechaCreacion = u.FechaCreacion.ToString("dd/MM/yyyy")
                })
                .ToListAsync();

            return ApiResponse<List<UsuarioDTO>>.Ok(usuarios);
        }

        public async Task<ApiResponse<List<RolDTO>>> GetRolesAsync()
        {
            var roles = await _db.Roles
                .Select(r => new RolDTO
                {
                    RolID = r.RolID,
                    Nombre = r.Nombre
                })
                .ToListAsync();

            return ApiResponse<List<RolDTO>>.Ok(roles);
        }

        public async Task<ApiResponse<UsuarioDTO>> CrearUsuarioAsync(CrearUsuarioRequest request)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Email == request.Email))
                return ApiResponse<UsuarioDTO>.Fail("El email ya está registrado");

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RolID = request.RolID,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();

            await _db.Entry(usuario).Reference(u => u.Rol).LoadAsync();

            return ApiResponse<UsuarioDTO>.Ok(new UsuarioDTO
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol!.Nombre,
                RolID = usuario.RolID,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion.ToString("dd/MM/yyyy")
            });
        }

        public async Task<ApiResponse<UsuarioDTO>> EditarUsuarioAsync(EditarUsuarioRequest request)
        {
            var usuario = await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == request.UsuarioID);

            if (usuario == null)
                return ApiResponse<UsuarioDTO>.Fail("Usuario no encontrado");

            if (await _db.Usuarios.AnyAsync(u => u.Email == request.Email && u.UsuarioID != request.UsuarioID))
                return ApiResponse<UsuarioDTO>.Fail("El email ya está en uso");

            usuario.Nombre = request.Nombre;
            usuario.Email = request.Email;
            usuario.RolID = request.RolID;
            usuario.Activo = request.Activo;

            await _db.SaveChangesAsync();
            await _db.Entry(usuario).Reference(u => u.Rol).LoadAsync();

            return ApiResponse<UsuarioDTO>.Ok(new UsuarioDTO
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol!.Nombre,
                RolID = usuario.RolID,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion.ToString("dd/MM/yyyy")
            });
        }

        public async Task<ApiResponse<bool>> ToggleActivoAsync(int usuarioId, bool activo)
        {
            var usuario = await _db.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return ApiResponse<bool>.Fail("Usuario no encontrado");

            usuario.Activo = activo;
            await _db.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, activo ? "Usuario activado" : "Usuario desactivado");
        }

        public async Task<ApiResponse<bool>> CambiarPasswordAsync(int usuarioId, CambiarPasswordRequest request)
        {
            var usuario = await _db.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return ApiResponse<bool>.Fail("Usuario no encontrado");

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NuevoPassword);
            await _db.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Contraseña actualizada correctamente");
        }
    }
}