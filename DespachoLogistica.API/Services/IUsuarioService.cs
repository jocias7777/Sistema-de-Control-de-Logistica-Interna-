using DespachoLogistica.API.Models.Common;
using DespachoLogistica.API.Models.DTOs;

namespace DespachoLogistica.API.Services
{
    public interface IUsuarioService
    {
        Task<ApiResponse<List<UsuarioDTO>>> GetUsuariosAsync();
        Task<ApiResponse<List<RolDTO>>> GetRolesAsync();
        Task<ApiResponse<UsuarioDTO>> CrearUsuarioAsync(CrearUsuarioRequest request);
        Task<ApiResponse<UsuarioDTO>> EditarUsuarioAsync(EditarUsuarioRequest request);
        Task<ApiResponse<bool>> ToggleActivoAsync(int usuarioId, bool activo);
        Task<ApiResponse<bool>> CambiarPasswordAsync(int usuarioId, CambiarPasswordRequest request);
    }
}
