using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AutoReservation.Dal.Entities
{
    public class Reservation
    {
        [Key, Column("ReservationsNr"), Required]
        public int ReservationsNr { get; set; }
        public int AutoId { get; set; }
        public int KundeId { get; set; }
        public DateTime Bis { get; set; }
        public DateTime Von { get; set; }
        public byte[]? RowVersion { get; set; }
        [ForeignKey(nameof(AutoId))]
        public virtual Auto Auto { get; set; }
        [ForeignKey(nameof(KundeId))]
        public virtual Kunde Kunde { get; set; }
    }
}