using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async void AddAuto(Auto auto)
        {
            await using var context = new AutoReservationContext();
            await context.Autos.AddAsync(auto);
            context.Entry(auto).State = EntityState.Added;
            await context.SaveChangesAsync();

        }

        public async void DeleteAuto(Auto auto)
        {
            await using var context = new AutoReservationContext();
            context.Autos.Remove(auto);
            context.Entry(auto).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        public async void ModifyAuto(Auto auto, int id, string marke, int tagestarif, int autoklasse)
        {
            await using var context = new AutoReservationContext();
            var toModify = context.Autos.Find(auto);
            toModify.Id = id;
            toModify.AutoKlasse = autoklasse;
            toModify.Marke = marke;
            toModify.Tagestarif = tagestarif;
            context.Entry(auto).State = EntityState.Modified;
            await context.SaveChangesAsync();

        }
    }
}