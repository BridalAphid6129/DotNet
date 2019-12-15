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
            // arrange
            // act
            // assert
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
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task GetAutoByIdWithIllegalIdTest()
        {
            const int invalidId = 1000;
            var request = new GetAutoRequest { IdFilter = invalidId };
            try
            {
                await _target.GetAutoAsync(request);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task InsertAutoTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Skoda Octavia", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            var insertResponse = await _target.InsertAutoAsync(autoToInsert);
            var getResponse = await _target.GetAutoAsync(new GetAutoRequest { IdFilter = insertResponse.Id });
            Assert.Equal(autoToInsert.Marke, getResponse.Marke);
            Assert.Equal(autoToInsert.Tagestarif, getResponse.Tagestarif);
            Assert.Equal(autoToInsert.AutoKlasse, getResponse.AutoKlasse);
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task DeleteAutoTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Skoda Octavia", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
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
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task UpdateAutoTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Skoda Octavia", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            const int newTagestarif = 55;

            var autoToUpdate = await _target.InsertAutoAsync(autoToInsert);
            autoToUpdate.Tagestarif = newTagestarif;
            var updateResponse = await _target.UpdateAutoAsync(autoToUpdate);

            Assert.Equal(autoToInsert.Marke, updateResponse.Marke);
            Assert.Equal(newTagestarif, updateResponse.Tagestarif);
            Assert.Equal(autoToInsert.AutoKlasse, updateResponse.AutoKlasse);
            // arrange
            // act
            // assert
        }

        [Fact]
        public async Task UpdateAutoWithOptimisticConcurrencyTest()
        {
            var autoToInsert = new AutoDto
            { Marke = "Skoda Octavia", Tagestarif = 50, AutoKlasse = AutoKlasse.Mittelklasse };
            const int newTagestarifA = 55;
            const int newTagestarifB = 45;

            var autoToUpdateA = await _target.InsertAutoAsync(autoToInsert);
            autoToUpdateA.Tagestarif = newTagestarifA;
            await _target.UpdateAutoAsync(autoToUpdateA);

            var autoToUpdateB = autoToUpdateA;
            autoToUpdateB.Tagestarif = newTagestarifB;
            try
            {
                await _target.UpdateAutoAsync(autoToUpdateB);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.Aborted, e.StatusCode);
            }
            // arrange
            // act
            // assert
        }
    }
}