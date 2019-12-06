using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public class LuxusklasseAuto : Auto
    {
        [Column("basetarif", TypeName = "DECIMAL")]
        public int? Basistarif { get; set; }
    }
}