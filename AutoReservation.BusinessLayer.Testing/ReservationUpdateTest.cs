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
            var bis = DateTime.Today;
            reservation.Bis = bis;
            await _target.UpdateReservation(reservation);
            var result = await _target.GetReservationById(1);
            Assert.Equal(bis, result.Bis);
        }
    }
}
