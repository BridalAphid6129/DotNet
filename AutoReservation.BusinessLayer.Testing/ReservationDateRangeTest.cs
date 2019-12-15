using System;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class ReservationDateRangeTest
        : TestBase
    {
        private readonly ReservationManager _target;

        public ReservationDateRangeTest()
        {
            _target = new ReservationManager();
        }

        [Fact]
        public async Task ScenarioOkay01TestAsync()
        {
            var dateVon = new DateTime(2019,12,14);
            var dateBis = new DateTime(2019,12,15);
            var reservation = await _target.GetReservationById(1);
            reservation.Von = dateVon;
            reservation.Bis = dateBis;
            await _target.UpdateReservation(reservation);
        }

        [Fact]
        public async Task ScenarioOkay02Test()
        {
            var dateBis = new DateTime(2019,12,24);
            var reservation = await _target.GetReservationById(1);
            reservation.Von = DateTime.Today;
            reservation.Bis = dateBis;
            await _target.UpdateReservation(reservation);
        }

        [Fact]
        public async Task ScenarioNotOkay01Test()
        {
            var reservation = await _target.GetReservationById(1);
            reservation.Von = DateTime.Today;
            reservation.Bis = DateTime.Today;
            await _target.UpdateReservation(reservation);
        }

        [Fact]
        public async Task ScenarioNotOkay02Test()
        {
            var reservation = await _target.GetReservationById(1);
            reservation.Bis = reservation.Von;
            await _target.UpdateReservation(reservation);
        }

        [Fact]
        public async Task ScenarioNotOkay03Test()
        {
            var testdate = new DateTime(2019, 12, 14);
            var reservation = await _target.GetReservationById(1);
            reservation.Von = testdate;
            reservation.Bis = testdate;
            await _target.UpdateReservation(reservation);
        }
    }
}
