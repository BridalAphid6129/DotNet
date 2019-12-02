using System;

namespace AutoReservation.Dal.Entities
{
    public class Reservation
    {
        public int AutoId { get; set; }
        public DateTime Bis { get; set; }
        public int KundeId { get; set; }
        public int ReservationsNr { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime Von { get; set; }
        
        public Kunde Kunde { get; set; }
        public Auto Auto { get; set; }
    }
}