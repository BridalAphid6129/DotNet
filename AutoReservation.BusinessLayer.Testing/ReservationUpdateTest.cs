using System;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class ReservationUpdateTest
        : TestBase
    {
        private readonly ReservationManager _target;

        public ReservationUpdateTest()
        {
            _target = new ReservationManager();
        }

        [Fact]
        public async Task UpdateReservationTest()
        {
            var reservation = await _target.GetReservationById(1);
            reservation.KundeId = 3;
            await _target.ModifyReservation(reservation);
            var result = await _target.GetReservationById(1);
            Assert.Equal(3, result.KundeId);
        }
    }
}
