using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AutoReservation.BusinessLayer
{
    public class AutoManager
        : ManagerBase
    {
        public async Task<List<Auto>> GetAllAutos()
        {
            using AutoReservationContext context = new AutoReservationContext();
            return await context.Autos.ToListAsync();
        }

        public async Task<Auto> GetAutoById(int autoId)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                var query = from c in context.Autos where c.Id == autoId
                            select c;
                var auto =
                context.Autos.Find(query);
                return auto;
            }
        }

        public async void AddAuto(int id, String marke, int tagestarif, int autoklasse)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                Auto auto = new StandardAuto()
                {
                    Tagestarif = tagestarif,
                    Id = id,
                    Marke = marke,
                    AutoKlasse = autoklasse
                };
                context.Autos.AddAsync(auto);
                context.Entry(auto).State = EntityState.Added;
                context.SaveChangesAsync();
            }
        }
        public async void DeleteAuto(Auto auto)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                context.Autos.Remove(auto);
                context.Entry(auto).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public async void ModifyAuto(Auto auto, int id, String marke, int tagestarif, int autoklasse)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                Auto toModify = context.Autos.Find(auto);
                toModify.Id = id;
                toModify.AutoKlasse = autoklasse;
                toModify.Marke = marke;
                toModify.Tagestarif = tagestarif;
                context.Entry(auto).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}