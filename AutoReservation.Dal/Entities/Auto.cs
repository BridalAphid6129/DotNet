using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace AutoReservation.Dal.Entities
{
    public abstract class Auto
    {
        public int Id { get; set; }
        public string Marke { get; set; }
        public byte[]? RowVersion { get; set; }
        public int Tagestarif { get; set; }
        public int AutoKlasse { get; set; }

        public ICollection<Reservation> Reservationen { get; set; }
    }
}