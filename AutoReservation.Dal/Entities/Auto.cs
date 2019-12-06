using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public abstract class Auto
    {
        [Key, Column("id")]
        public int Id { get; set; }
        [Column("marke"), Required]

        public string Marke { get; set; }
        [Column("row")]

        public byte[] RowVersion { get; set; }
        [Column("tarif", TypeName = "DECIMAL")]

        public int? Tagestarif { get; set; }
    }
}