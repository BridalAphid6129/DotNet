using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AutoReservation.BusinessLayer
{
    public class ReservationManager
        : ManagerBase
    {
        public async Task<List<Reservation>> GetAllReservationen()
        {
            await using var context = new AutoReservationContext();
            return await context.Reservationen.ToListAsync();
        }
        public async Task<Reservation> GetReservationById(int id)
        {
            await using var context = new AutoReservationContext();
            var query = from c in context.Reservationen
                where c.ReservationsNr == id
                select c;
            return context.Reservationen.FindAsync(query).Result;
        }

        public async Task<Reservation> AddReservation(Reservation reservation)
        {
            await using var context = new AutoReservationContext();
            if (AvailabilityCheck(reservation).Result)
            {
                throw new AutoUnavailableException("Auto unavailable during Von to Bis");
            }

            if (DateRangeCheck(reservation).Result)
            {
                throw new InvalidDateRangeException("Incorrect Dates");
            }

            context.Entry(reservation).State = EntityState.Added;
            await context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> DeleteReservation(Reservation reservation)
        {
            await using var context = new AutoReservationContext();
            context.Entry(reservation).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return reservation;
        }

        public async Task<Reservation> ModifyReservation(Reservation reservation)
        {
            try
            {
                await using var context = new AutoReservationContext();
                if (!AvailabilityCheck(reservation).Result)
                {
                    throw new AutoUnavailableException("Auto unavailable during Von to Bis");
                }
                if (DateRangeCheck(reservation).Result)
                {
                    throw new InvalidDateRangeException("Incorrect Dates");
                }
                context.Entry(reservation).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new OptimisticConcurrencyException<Reservation>("failed to create: ", reservation);
            }
            return reservation;
        }

        private static async Task<bool> AvailabilityCheck(Reservation createreservation)
        {
            await using var context = new AutoReservationContext();
            return (Enumerable.Any(
                context.Reservationen.Where(reservation => createreservation.AutoId == reservation.AutoId),
                reservation => createreservation.Von > reservation.Bis));
        }

        private static async Task<bool> DateRangeCheck(Reservation reservation)
        {
            await using var context = new AutoReservationContext();
            return reservation.Von == reservation.Bis;
        }
    }
}