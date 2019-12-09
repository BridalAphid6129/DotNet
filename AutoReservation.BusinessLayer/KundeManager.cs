using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoReservation.BusinessLayer
{
    public class KundeManager
        : ManagerBase
    { 
        public async Task<List<Kunde>> GetAllKunden()
        {
            using AutoReservationContext context = new AutoReservationContext();
            return await context.Kunden.ToListAsync();
        }
        public async Task<Kunde> GetKundeById(int id)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                var query = from c in context.Kunden
                    where c.Id == id
                    select c;
                return await context.Kunden.FindAsync(query);
            }
        }

        public async void AddKunde(Kunde kunde)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                await context.Kunden.AddAsync(kunde);
                context.Entry(kunde).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }
        public async void DeleteKunde(Kunde kunde)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                context.Kunden.Remove(kunde);
                context.Entry(kunde).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }
        public async void ModifyKunde(Kunde kunde, int id, DateTime geburtstag, String nachname, String vorname)
        {
            using (AutoReservationContext context = new AutoReservationContext())
            {
                Kunde toModify = context.Kunden.Find(kunde);
                toModify.Id = id;
                toModify.Geburtsdatum = geburtstag;
                toModify.Nachname = nachname;
                toModify.Vorname = vorname;
                context.Entry(kunde).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}