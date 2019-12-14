using System.Threading.Tasks;
using AutoReservation.TestEnvironment;
using Xunit;

namespace AutoReservation.BusinessLayer.Testing
{
    public class AutoUpdateTests
        : TestBase
    {
        private readonly AutoManager _target;

        public AutoUpdateTests()
        {
            _target = new AutoManager();
        }
        [Fact]
        public async Task UpdateAutoTest()
        {
            var auto = await _target.GetAutoById(3);
            auto.Marke = "Lada";
            await _target.ModifyAuto(auto);
            var result = await _target.GetAutoById(3);
            Assert.Equal("Lada", result.Marke);
        }
    }
}
