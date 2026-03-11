using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DespachoLogistica.API.Models.Entities
{
    [Table("Stock")]
    public class Stock
    {
        [Key]
        public int StockID { get; set; }

        public int ProductoID { get; set; }
        public int BodegaID { get; set; }
        public decimal Cantidad { get; set; } = 0;

        [ForeignKey("ProductoID")]
        public virtual Producto? Producto { get; set; }

        [ForeignKey("BodegaID")]
        public virtual Bodega? Bodega { get; set; }
    }
}