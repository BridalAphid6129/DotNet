using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace AutoReservation.BusinessLayer
{
    public class KundeManager
        : ManagerBase
    { 
        public async Task<List<Kunde>> GetAllKunden()
        {
            await using var context = new AutoReservationContext();
            return await context.Kunden.ToListAsync();
        }

        public async Task<Kunde> GetKundeById(int id)
        {
            await using var context = new AutoReservationContext();
                var query = from c in context.Kunden
                    where c.Id == id
                    select c;
            return await context.Kunden.FindAsync(query);
        }

        public async Task<Kunde> AddKunde(Kunde kunde)
        {
            await using var context = new AutoReservationContext();
            context.Entry(kunde).State = EntityState.Added;
            await context.SaveChangesAsync();
            return kunde;
        }

        public async Task<Kunde> DeleteKunde(Kunde kunde)
        {
            await using var context = new AutoReservationContext();
            context.Entry(kunde).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return kunde;
        }

        public async Task<Kunde> UpdateKunde(Kunde kunde)
        {
            try
            {
                await using var context = new AutoReservationContext();
                context.Entry(kunde).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return kunde;
            }
            catch (Exception e)
            {
                throw new OptimisticConcurrencyException<Kunde>("failed to create: ", kunde);
            }
        }
    }
}