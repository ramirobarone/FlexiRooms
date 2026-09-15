using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class HotelInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? TerminosYCondiciones { get; set; }
        public string? InstruccionesDeUso { get; set; }
        public int HotelId { get; set; }
        public virtual Hotel? Hotel { get; set; }
    }
}
