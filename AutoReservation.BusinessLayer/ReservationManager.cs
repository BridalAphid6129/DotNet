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
    }
}