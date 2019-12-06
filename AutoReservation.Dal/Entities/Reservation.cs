using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoReservation.Dal.Entities
{
    public class Reservation
    {
        [Column("AutoId"), Required]
        public int AutoId { get; set; }
        [Column("Bis", TypeName = "DATETIME")]

        public DateTime Bis { get; set; }
        [Column("KundenId"), Required]

        public int KundeId { get; set; }
        [Key, Column("ReservationsNr")]
        public int ReservationsNr { get; set; }
        [Column("row")]
        public byte[] RowVersion { get; set; }
        [Column("Von", TypeName = "DATETIME")]
        public DateTime Von { get; set; }

        [ForeignKey(nameof(AutoId))] 
        public virtual Auto Auto { get; set; }
        [ForeignKey(nameof(KundeId))]
        public virtual Kunde Kunde { get; set; }
    }
}