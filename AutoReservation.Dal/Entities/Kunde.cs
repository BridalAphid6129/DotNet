using System;

namespace AutoReservation.Dal.Entities
{
    public class Kunde
    {
        public DateTime Geburtsdatum { get; set; }
        public int Id { get; set; }
        public string Nachname { get; set; }
        public byte[] RowVersion { get; set; }
        public string Vorname { get; set; }
    }
}