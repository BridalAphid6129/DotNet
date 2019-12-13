using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoReservation.Dal
{
    public class AutoReservationContext
        : AutoReservationContextBase
    { 
      public DbSet<Auto> Autos { get; set; }
      public DbSet<Kunde> Kunden { get; set; }
      public DbSet<Reservation> Reservationen { get; set; }

    }
}
  