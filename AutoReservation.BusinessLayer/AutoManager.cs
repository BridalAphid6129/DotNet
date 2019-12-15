using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoReservation.BusinessLayer
{
    public class AutoManager
        : ManagerBase
    {
        public async Task<List<Auto>> GetAllAutos()
        {
            await using var context = new AutoReservationContext();
            return await context.Autos.ToListAsync();
        }

        public async Task<Auto> GetAutoById(int id)
        {
            await using var context = new AutoReservationContext();
            return await context.Autos.FindAsync(id);
        }

        public async Task<Auto> AddAuto(Auto auto)
        {
            await using var context = new AutoReservationContext();
            context.Entry(auto).State = EntityState.Added;
            await context.SaveChangesAsync();
            return auto;
        }

        public async Task<T> DeleteAuto <T>(T auto)
        {
            await using var context = new AutoReservationContext();
            context.Entry(auto).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return auto;
        }

        public async Task<Auto> ModifyAuto(Auto auto)
        {
            await using var context = new AutoReservationContext();
            try
            {
                context.Entry(auto).State = EntityState.Modified; 
                await context.SaveChangesAsync();
                return auto;
            }
            catch (Exception e)
            {
                throw new OptimisticConcurrencyException<Auto>("failed to update: ", auto);
            }
        }
    }
}