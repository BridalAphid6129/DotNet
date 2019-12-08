using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public class LuxusklasseAuto : Auto
    {
        public int? Basistarif { get; set; }
    }
}