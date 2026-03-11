using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DespachoLogistica.API.Models.Entities
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }

        [Required, MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required, MaxLength(50)]
        public string UnidadMedida { get; set; } = string.Empty;

        public decimal StockMinimo { get; set; } = 0;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}