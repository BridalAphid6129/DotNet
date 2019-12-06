using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoReservation.Dal
{
    public class AutoReservationContext
        : AutoReservationContextBase
    {
      public virtual DbSet<Auto> Autos { get; set; }
      public virtual DbSet<Kunde> Kunden { get; set; }
      public virtual DbSet<Reservation> Reservationen { get; set; }
    }
}
