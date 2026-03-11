using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DespachoLogistica.API.Models.Entities
{
    [Table("SolicitudDetalles")]
    public class SolicitudDetalle
    {
        [Key]
        public int DetalleID { get; set; }
        public int SolicitudID { get; set; }
        public int ProductoID { get; set; }
        public decimal Cantidad { get; set; }

        [ForeignKey("SolicitudID")]
        public virtual Solicitud? Solicitud { get; set; }
        [ForeignKey("ProductoID")]
        public virtual Producto? Producto { get; set; }
    }
}