using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DespachoLogistica.API.Models.Entities
{
    [Table("Roles")]
    public class Rol
    {
        [Key]
        public int RolID { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}