using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DespachoLogistica.API.Models.Entities
{
    [Table("Bodegas")]
    public class Bodega
    {
        [Key]
        public int BodegaID { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Ubicacion { get; set; }

        public bool Activa { get; set; } = true;

        public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}