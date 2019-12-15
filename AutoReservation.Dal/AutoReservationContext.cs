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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
    .Entity<LuxusklasseAuto>()
    .HasBaseType<Auto>();

            builder
                .Entity<MittelklasseAuto>()
                .HasBaseType<Auto>();
            builder
                .Entity<StandardAuto>()
                .HasBaseType<Auto>();
        }
    }
}
  