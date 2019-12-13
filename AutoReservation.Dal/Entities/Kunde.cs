using System;
using System.Collections.Generic;

namespace AutoReservation.Dal.Entities
{
    public class Kunde
    {
        public int Id { get; set; }
        public DateTime Geburtsdatum { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public byte[]? RowVersion { get; set; }

        public ICollection<Reservation> Reservationen { get; set; }

    }
}