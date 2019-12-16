using System;
using System.Threading.Tasks;
using AutoReservation.Service.Grpc.Testing.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Xunit;

namespace AutoReservation.Service.Grpc.Testing
{
    public class AutoServiceTests
        : ServiceTestBase
    {
        private readonly AutoService.AutoServiceClient _target;

        public AutoServiceTests(ServiceTestFixture serviceTestFixture)
            : base(serviceTestFixture)
        {
            _target = new AutoService.AutoServiceClient(Channel);
        }


        [Fact]
        public async Task GetAutosTest()
        {
            var request = new Empty();
            var response = await _target.GetAutosAsync(request);
            Assert.Equal(4, response.Items.Count);
        }

        [Fact]
        public async Task GetAutoByIdTest()
        {
            const int id = 1;
            var request = new GetAutoRequest { IdFilter = id };
            var response = await _target.GetAutoAsync(request);
            Assert.Equal(id, response.Id);
            Assert.Equal("Fiat Punto", response.Marke);
            Assert.Equal(50, response.Tagestarif);
        }

        [Fact]
        public async Task GetAutoByIdWithIllegalIdTest()
        {
            const int invalidId = 100;
            var request = new GetAutoRequest { IdFilter = invalidId };
            try
            {
                await _target.GetAutoAsync(request);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
        }

        [Fact]
        public async Task InsertAutoTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Opel", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            var insertedAuto = await _target.InsertAutoAsync(autoToInsert);
            var selectedAuto = await _target.GetAutoAsync(new GetAutoRequest { IdFilter = insertedAuto.Id });
            Assert.Equal(autoToInsert.Marke, selectedAuto.Marke);
            Assert.Equal(autoToInsert.Tagestarif, selectedAuto.Tagestarif);
            Assert.Equal(autoToInsert.AutoKlasse, selectedAuto.AutoKlasse);
        }

        [Fact]
        public async Task DeleteAutoTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Opel", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            var autoToDelete = await _target.InsertAutoAsync(autoToInsert);
            await _target.DeleteAutoAsync(autoToDelete);
            try
            {
                await _target.GetAutoAsync(new GetAutoRequest { IdFilter = autoToDelete.Id });
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
        }

        [Fact]
        public async Task UpdateAutoTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Opel", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            const int newTagestarif = 100;

            var autoToUpdate = await _target.InsertAutoAsync(autoToInsert);
            autoToUpdate.Tagestarif = newTagestarif;
            var updatedAuto = await _target.UpdateAutoAsync(autoToUpdate);

            Assert.Equal(autoToInsert.Marke, updatedAuto.Marke);
            Assert.Equal(newTagestarif, updatedAuto.Tagestarif);
            Assert.Equal(autoToInsert.AutoKlasse, updatedAuto.AutoKlasse);
        }

        [Fact]
        public async Task UpdateAutoWithOptimisticConcurrencyTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Opel", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            const int newTagestarifA = 100;
            const int newTagestarifB = 10;

            var autoAToUpdate = await _target.InsertAutoAsync(autoToInsert);
            autoAToUpdate.Tagestarif = newTagestarifA;
            await _target.UpdateAutoAsync(autoAToUpdate);

            var autoBToUpdate = autoAToUpdate;
            autoBToUpdate.Tagestarif = newTagestarifB;
            try
            {
                await _target.UpdateAutoAsync(autoBToUpdate);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.Aborted, e.StatusCode);
            }
        }
    }
}