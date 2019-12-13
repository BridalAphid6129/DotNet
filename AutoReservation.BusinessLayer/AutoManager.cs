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

        public async Task<Auto> GetAutoById(int autoId)
        {
            await using var context = new AutoReservationContext();
            var query = from c in context.Autos where c.Id == autoId
                            select c;
            return await context.Autos.FindAsync(query);
        }
        public async Task AddAuto(Auto auto)
        {
            try
            {
                await using var context = new AutoReservationContext();
                context.Entry(auto).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new OptimisticConcurrencyException<Auto>("failed to create: ", auto);
            }
        }

        public async Task DeleteAuto(Auto auto)
        {
            try
            {
                await using var context = new AutoReservationContext();
                context.Entry(auto).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new OptimisticConcurrencyException<Auto>("failed to delete: ", auto);
            } 
        }

        public async Task ModifyAuto(Auto auto)
        {
            await using var context = new AutoReservationContext();
            try
            {
                context.Entry(auto).State = EntityState.Modified; 
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new OptimisticConcurrencyException<Auto>("failed to update: ", auto);
            }
        }
    }
}