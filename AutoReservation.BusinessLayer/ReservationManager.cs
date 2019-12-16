using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
            return await context.Reservationen.FindAsync(id);
        }

        public async Task<Reservation> AddReservation(Reservation reservation)
        {
            await using var context = new AutoReservationContext();
            if (await AvailabilityCheck(reservation))
            {
                throw new AutoUnavailableException("Auto unavailable during Von to Bis");
            }

            if (await DateRangeCheck(reservation))
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

        public async Task<Reservation> UpdateReservation(Reservation reservation)
        {
            try
            {
                await using var context = new AutoReservationContext();
                if (await AvailabilityCheck(reservation))
                {
                    throw new AutoUnavailableException("Auto unavailable during Von to Bis");
                }
                if (await DateRangeCheck(reservation))
                {
                    throw new InvalidDateRangeException("Incorrect Dates");
                }
                context.Entry(reservation).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return reservation;
            }
            catch (Exception e)
            {
                if (await DateRangeCheck(reservation))
                {
                    throw new InvalidDateRangeException("Incorrect Dates");
                }
                if (await AvailabilityCheck(reservation))
                {
                    throw new AutoUnavailableException("Auto unavailable during Von to Bis");
                }
                throw new OptimisticConcurrencyException<Reservation>("failed to create: ", reservation);
            }
        }

        private static async Task<bool> AvailabilityCheck(Reservation createReservation)
        {
            await using var context = new AutoReservationContext();
            var reservations = context.Reservationen.Where(r =>
                r.AutoId == createReservation.AutoId && r.ReservationsNr != createReservation.ReservationsNr);
            foreach (var reservation in reservations)
            {
                if (reservation.AutoId == createReservation.AutoId && 
                    (reservation.Von < createReservation.Bis && createReservation.Von < reservation.Bis))
                {
                    throw new AutoUnavailableException("Auto unavailable");
                }    
            }
            return true;
        }

        private static async Task<bool> DateRangeCheck(Reservation reservation)
        {
            await using var context = new AutoReservationContext();
            return reservation.Von == reservation.Bis;
        }
    }
}