using System;
using System.Threading.Tasks;
using AutoReservation.Dal;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class KundeUpdateTest
        : TestBase
    {
        private readonly KundeManager _target;

        public KundeUpdateTest()
        {
            _target = new KundeManager();
        }
        
        [Fact]
        public async Task UpdateKundeTest()
        {
            await using var AutoReservationContext = new AutoReservationContext();
            var kunde = await _target.GetKundeById(1);
            kunde.Vorname = "Elfriede";
            await _target.UpdateKunde(kunde);
            var result = await _target.GetKundeById(1);
            Assert.Equal("Elfriede", result.Vorname);
        }
    }
}
