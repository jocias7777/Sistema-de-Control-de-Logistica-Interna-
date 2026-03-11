using System.ComponentModel.DataAnnotations;

namespace DespachoLogistica.API.Models.Entities
{
    public class KardexMovimiento
    {
        [Key]  // ✅ Esto faltaba
        public int MovimientoID { get; set; }
        public int ProductoID { get; set; }
        public int BodegaID { get; set; }
        public string Tipo { get; set; } = ""; // "Entrada" | "Salida"
        public decimal Cantidad { get; set; }
        public decimal StockAntes { get; set; }
        public decimal StockDespues { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioID { get; set; }
        public string Referencia { get; set; } = "";
        public string Observacion { get; set; } = "";

        // Navegación
        public Producto? Producto { get; set; }
        public Bodega? Bodega { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
