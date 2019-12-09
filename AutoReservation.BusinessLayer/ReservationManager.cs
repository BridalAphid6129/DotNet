using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoReservation.BusinessLayer
{
    public class ReservationManager
        : ManagerBase
    {
        public async Task<List<Reservation>> GetAllReservationen()
        {
            using AutoReservationContext context = new AutoReservationContext();
            return await context.Reservationen.ToListAsync();
        }
        public async Task<Reservation> GetReservationById(int id)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                var query = from c in context.Reservationen
                    where c.ReservationsNr == id
                    select c; 
                return await context.Reservationen.FindAsync(query);
            }
        }

        public async void AddReservation(Reservation reservation)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                await context.Reservationen.AddAsync(reservation);
                context.Entry(reservation).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }
        public async void DeleteReservation(Reservation reservation)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                context.Reservationen.Remove(reservation);
                context.Entry(reservation).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }
        public async void ModifyReservation(Reservation reservation, int id, int autoid, int kundenid, DateTime von, DateTime bis, Auto auto , Kunde kunde)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                Reservation toModify = context.Reservationen.Find(reservation);
                toModify.ReservationsNr = id;
                toModify.Von = von;
                toModify.Bis = bis;
                toModify.Auto = auto;
                toModify.Kunde = kunde;
                toModify.AutoId = autoid;
                toModify.KundeId = kundenid;
                context.Entry(reservation).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}