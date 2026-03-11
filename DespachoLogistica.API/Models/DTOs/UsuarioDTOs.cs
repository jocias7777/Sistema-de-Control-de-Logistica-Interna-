namespace DespachoLogistica.API.Models.DTOs
{
    public class UsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int RolID { get; set; }
        public bool Activo { get; set; }
        public string FechaCreacion { get; set; } = string.Empty;
    }

    public class CrearUsuarioRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolID { get; set; }
    }

    public class EditarUsuarioRequest
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RolID { get; set; }
        public bool Activo { get; set; }
    }

    public class CambiarPasswordRequest
    {
        public string NuevoPassword { get; set; } = string.Empty;
    }

    public class RolDTO
    {
        public int RolID { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}