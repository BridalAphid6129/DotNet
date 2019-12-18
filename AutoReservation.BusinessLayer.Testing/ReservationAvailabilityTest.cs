using System;
using System.Threading.Tasks;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class ReservationAvailabilityTest
        : TestBase
    {
        private readonly ReservationManager _target;
        public ReservationAvailabilityTest()
        {
            _target = new ReservationManager();
        }

        [Fact]
        public async Task ScenarioOkay01Test()
        {
            var reservation1 = await _target.GetReservationById(1);
            //| ---Date 1--- |
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2019, 12, 24);
            //               | ---Date 2--- |
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 12, 24 );
            var date2bis = new DateTime(2020, 1, 5);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            // act
            async Task Act1() => await _target.UpdateReservation(reservation1);
            async Task Act2() => await _target.UpdateReservation(reservation2);
            // assert
            Assert.Equal(reservation2.Von, reservation1.Bis);
        }

        [Fact]
        public async Task ScenarioOkay02Test()
        {
            // arrange
            //| ---Date 1--- |
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2019, 12, 24);
            //                 | ---Date 2--- |
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 12, 31 );
            var date2bis = new DateTime(2020, 1, 5);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            // act
            async Task Act1() => await _target.UpdateReservation(reservation1);
            async Task Act2() => await _target.UpdateReservation(reservation2);
            // assert
            Assert.Equal(Act1().IsCompleted,Act2().IsCompleted);
        }

        [Fact]
        public async Task ScenarioOkay03Test()
        {
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2020, 1, 5);
            var date1bis = new DateTime(2020, 5, 13);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation1.AutoId = 1;
            await _target.UpdateReservation(reservation1);
            
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 11, 5 );
            var date2bis = new DateTime(2020, 1, 5);
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            reservation2.AutoId = 1;
            
            Assert.Equal(new DateTime(2020, 1,5),reservation1.Von);
            Assert.Equal(new DateTime(2020, 5,13),reservation1.Bis);
            Assert.Equal(new DateTime(2019, 11,5),reservation2.Von);
            Assert.Equal(new DateTime(2020, 1,5),reservation2.Bis);
            // arrange
            //                | ---Date 1--- |
            //| ---Date 2-- - |
            // act
            // assert
        }

        [Fact]
        public async Task ScenarioOkay04Test()
        {
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2019, 12, 24);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation1.AutoId = 1;
            await _target.UpdateReservation(reservation1);
            
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 12, 24 );
            var date2bis = new DateTime(2020, 1, 5);
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            reservation2.AutoId = 1;
            
            Assert.Equal(new DateTime(2019, 10,20),reservation1.Von);
            Assert.Equal(new DateTime(2019, 12,24),reservation1.Bis);
            Assert.Equal(new DateTime(2019, 12,24),reservation2.Von);
            Assert.Equal(new DateTime(2020, 1,5),reservation2.Bis);

        }

        [Fact]
        public async Task ScenarioNotOkay01Test()
        {
            //| ---Date 1--- |
            //    | ---Date 2--- |
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2019, 12, 24);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation1.AutoId = 1;
            await _target.UpdateReservation(reservation1);
            
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 11, 5 );
            var date2bis = new DateTime(2020, 1, 5);
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            reservation2.AutoId = 1;
            
            async Task Act() => await _target.UpdateReservation(reservation2);
            
            await Assert.ThrowsAsync<AutoUnavailableException>(Act);
        }

        [Fact]
        public async Task ScenarioNotOkay02Test()
        {
            //    | ---Date 1--- |
            //| ---Date 2--- |
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 12, 24); 
            var date1bis = new DateTime(2020, 1, 6);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation1.AutoId = 1;
            await _target.UpdateReservation(reservation1);
            
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 12, 2 );
            var date2bis = new DateTime(2020, 12, 30);
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            reservation2.AutoId = 1;
            
            async Task Act() => await _target.UpdateReservation(reservation2);
            
            await Assert.ThrowsAsync<AutoUnavailableException>(Act);
        }

        [Fact]
        public async Task ScenarioNotOkay03Test()
        {
            //| ---Date 1--- |
            //| --------Date 2-------- |
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2019, 10, 27);
            reservation1.AutoId = 1;
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            await _target.UpdateReservation(reservation1);
            
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 10, 20 );
            var date2bis = new DateTime(2020, 1, 6);
            reservation2.AutoId = 1;
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            
            async Task Act() => await _target.UpdateReservation(reservation2);
            
            await Assert.ThrowsAsync<AutoUnavailableException>(Act);
        }

        [Fact]
        public async Task ScenarioNotOkay04Test()
        {
            //| --------Date 1-------- |
            //| ---Date 2--- |
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2020, 3, 15);
            reservation1.AutoId = 1;
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            await _target.UpdateReservation(reservation1);
            
            var reservation2 = await _target.GetReservationById(2);
            var date2von = new DateTime(2019, 10, 20 );
            var date2bis = new DateTime(2019, 12, 28);
            reservation2.AutoId = 1;
            reservation2.Von = date2von;
            reservation2.Bis = date2bis;
            async Task Act() => await _target.UpdateReservation(reservation2);
            
            await Assert.ThrowsAsync<AutoUnavailableException>(Act);
        }

        [Fact]
        public async Task ScenarioNotOkay05Test()
        {
            //| ---Date 1--- |
            //| ---Date 2--- |
            var reservation1 = await _target.GetReservationById(1);
            var date1von = new DateTime(2019, 10, 20);
            var date1bis = new DateTime(2019, 12, 24);
            reservation1.Von = date1von;
            reservation1.Bis = date1bis;
            reservation1.AutoId = 1;
            await _target.UpdateReservation(reservation1);
                
            var reservation2 = await _target.GetReservationById(2);
            reservation2.Von = reservation1.Von;
            reservation2.Bis = reservation1.Bis;
            reservation2.AutoId = 1;
                
            async Task Act() => await _target.UpdateReservation(reservation2);
            
            await Assert.ThrowsAsync<AutoUnavailableException>(Act);
        }
    }
}
