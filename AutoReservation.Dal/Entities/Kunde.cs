using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AutoReservation.Dal.Entities
{
    public class Kunde
    {
        public int Id { get; set; }
        public DateTime Geburtsdatum { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<Reservation> Reservationen { get; set; }

    }
}