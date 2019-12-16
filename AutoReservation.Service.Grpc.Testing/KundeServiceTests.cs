using System;
using System.Threading.Tasks;
using AutoReservation.Service.Grpc.Testing.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Xunit;

namespace AutoReservation.Service.Grpc.Testing
{
    public class KundeServiceTests
        : ServiceTestBase
    {
        private readonly KundeService.KundeServiceClient _target;

        public KundeServiceTests(ServiceTestFixture serviceTestFixture)
            : base(serviceTestFixture)
        {
            _target = new KundeService.KundeServiceClient(Channel);
        }

        [Fact]
        public async Task GetKundenTest()
        {
            var request = new Empty();
            var response = await _target.GetKundenAsync(request);
            Assert.Equal(4, response.Items.Count);
        }

        [Fact]
        public async Task GetKundeByIdTest()
        {
            const int id = 1;
            var request = new GetKundeRequest { IdFilter = id };
            var response = await _target.GetKundeAsync(request);
            Assert.Equal(id, response.Id);
            Assert.Equal("Nass", response.Nachname);
            Assert.Equal("Anna", response.Vorname);
        }

        [Fact]
        public async Task GetKundeByIdWithIllegalIdTest()
        {
            const int invalidId = 100;
            var request = new GetKundeRequest { IdFilter = invalidId };
            try
            {
                await _target.GetKundeAsync(request);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
        }

        [Fact]
        public async Task InsertKundeTest()
        {
            var kundeToInsert = new KundeDto
            {
                Vorname = "foo",
                Nachname = "bar",
                Geburtsdatum = DateTime.UtcNow.ToTimestamp()
            };
            var insertedKunde = await _target.InsertKundeAsync(kundeToInsert);
            var selectedKunde = await _target.GetKundeAsync(new GetKundeRequest { IdFilter = insertedKunde.Id });
            Assert.Equal(kundeToInsert.Vorname, selectedKunde.Vorname);
            Assert.Equal(kundeToInsert.Nachname, selectedKunde.Nachname);
            Assert.Equal(kundeToInsert.Geburtsdatum, selectedKunde.Geburtsdatum);
        }

        [Fact]
        public async Task DeleteKundeTest()
        {
            var kundToInsert = new KundeDto
            {
                Geburtsdatum = DateTime.UtcNow.ToTimestamp(),
                Nachname = "foo",
                Vorname = "bar"
            };
            var kundeToDelete = await _target.InsertKundeAsync(kundToInsert);
            await _target.DeleteKundeAsync(kundeToDelete);
            try
            {
                await _target.GetKundeAsync(new GetKundeRequest { IdFilter = kundeToDelete.Id });
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
        }

        [Fact]
        public async Task UpdateKundeTest()
        {
            var kundeToInsert = new KundeDto
            {
                Vorname = "foo",
                Nachname = "bar",
                Geburtsdatum = DateTime.UtcNow.ToTimestamp()
            };
            Timestamp newGeburtsdatum = DateTime.UtcNow.AddDays(1).ToTimestamp();

            var kundeToUpdate = await _target.InsertKundeAsync(kundeToInsert);
            kundeToUpdate.Geburtsdatum = newGeburtsdatum;
            var updatedKunde = await _target.UpdateKundeAsync(kundeToUpdate);

            Assert.Equal(kundeToInsert.Vorname, updatedKunde.Vorname);
            Assert.Equal(newGeburtsdatum, updatedKunde.Geburtsdatum);
            Assert.Equal(kundeToInsert.Nachname, updatedKunde.Nachname);
        }

        [Fact]
        public async Task UpdateKundeWithOptimisticConcurrencyTest()
        {
            var kundeToInsert = new KundeDto
            {
                Vorname = "foo",
                Nachname = "bar",
                Geburtsdatum = DateTime.UtcNow.ToTimestamp()
            };

            Timestamp newGeburtsdatumA = DateTime.UtcNow.AddDays(1).ToTimestamp();
            Timestamp newGeburtsdatumB = DateTime.UtcNow.AddDays(2).ToTimestamp();

            var kundeAToUpdate = await _target.InsertKundeAsync(kundeToInsert);
            kundeAToUpdate.Geburtsdatum = newGeburtsdatumA;
            await _target.UpdateKundeAsync(kundeAToUpdate);

            var kundeBToUpdate = kundeAToUpdate;
            kundeBToUpdate.Geburtsdatum = newGeburtsdatumB;
            try
            {
                await _target.UpdateKundeAsync(kundeBToUpdate);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.Aborted, e.StatusCode);
            }
        }
    }
}