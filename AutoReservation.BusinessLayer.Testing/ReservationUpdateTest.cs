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
            var reservation = await _target.GetReservationById(4);
            var von = new DateTime(2019, 12, 24);
            var bis = new DateTime(2019, 12, 28);
            reservation.Von = von;
            reservation.Bis = bis;
            await _target.UpdateReservation(reservation);
            var result = await _target.GetReservationById(4);
            Assert.Equal(von, result.Von);
            Assert.Equal(bis, result.Bis);
        }
    }
}
