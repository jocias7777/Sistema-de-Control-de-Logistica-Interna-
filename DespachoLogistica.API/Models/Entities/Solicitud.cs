using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DespachoLogistica.API.Models.Entities
{
    [Table("Solicitudes")]
    public class Solicitud
    {
        [Key]
        public int SolicitudID { get; set; }
        public string NumeroSolicitud { get; set; } = string.Empty;
        public int SolicitanteID { get; set; }
        public int BodegaID { get; set; }
        public string Estado { get; set; } = "Borrador";
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey("SolicitanteID")]
        public virtual Usuario? Solicitante { get; set; }
        [ForeignKey("BodegaID")]
        public virtual Bodega? Bodega { get; set; }
        public virtual ICollection<SolicitudDetalle> Detalles { get; set; } = new List<SolicitudDetalle>();
    }
}