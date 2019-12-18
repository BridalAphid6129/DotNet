using System;
using System.Threading.Tasks;
using AutoReservation.Service.Grpc.Testing.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Xunit;

namespace AutoReservation.Service.Grpc.Testing
{
    public class ReservationServiceTests
        : ServiceTestBase
    {
        private readonly ReservationService.ReservationServiceClient _target;
        private readonly AutoService.AutoServiceClient _autoClient;
        private readonly KundeService.KundeServiceClient _kundeClient;

        public ReservationServiceTests(ServiceTestFixture serviceTestFixture)
            : base(serviceTestFixture)
        {
            _target = new ReservationService.ReservationServiceClient(Channel);
            _autoClient = new AutoService.AutoServiceClient(Channel);
            _kundeClient = new KundeService.KundeServiceClient(Channel);
        }

        [Fact]
        public async Task GetReservationenTest()
        {
            var empty = new Empty();
            var reservationen = await _target.GetReservationenAsync(empty);
            Assert.Equal(4, reservationen.Items.Count);
        }

        [Fact]
        public async Task GetReservationByIdTest()
        {
            var request = new GetReservationRequest { Id = 1 };
            var response = await _target.GetReservationenByIdAsync(request);
            Assert.Equal(1, response.ReservationsNr);
        }

        [Fact]
        public async Task GetReservationByIdWithIllegalIdTest()
        {
            const int invalidId = 100;
            var request = new GetReservationRequest { Id = invalidId };
            try
            {
                await _target.GetReservationenByIdAsync(request);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
        }

        [Fact]
        public async Task InsertReservationTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp() ,
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto() { Marke = "foo", Basistarif = 20, AutoKlasse = AutoKlasse.Luxusklasse},
                Kunde = new KundeDto() { Vorname = "foo", Nachname = "bar", Geburtsdatum = DateTime.UtcNow.ToTimestamp()}
            };
            var insertedReservation = await _target.InsertReservationAsync(reservationToInsert);
            var selectedReservation = await _target.GetReservationenByIdAsync(new GetReservationRequest { Id = insertedReservation.ReservationsNr });
            Assert.Equal(reservationToInsert.Von, selectedReservation.Von);
            Assert.Equal(reservationToInsert.Bis, selectedReservation.Bis);
        }

        [Fact]
        public async Task DeleteReservationTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto() { Id = 3 },
                Kunde = new KundeDto() { Id = 3 }
            };
            var reservationToDelete = await _target.InsertReservationAsync(reservationToInsert);
            reservationToDelete.Auto = new AutoDto{ Id = 3};
            reservationToDelete.Kunde = new KundeDto { Id = 3 };
            var deletedReservation = await _target.DeleteReservationAsync(reservationToDelete);
            try
            {
                await _target.GetReservationenByIdAsync(new GetReservationRequest { Id = reservationToDelete.ReservationsNr });
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.NotFound, e.StatusCode);
            }
        }

        [Fact]
        public async Task UpdateReservationTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 1 }
            };
            var reservationToUpdate = await _target.InsertReservationAsync(reservationToInsert);
            var newDate = DateTime.UtcNow.AddMinutes(1).ToTimestamp();
            reservationToUpdate.Von = newDate;
            reservationToUpdate.Auto = new AutoDto { Id = 2 };
            reservationToUpdate.Kunde = new KundeDto { Id = 2 };
            var updatedReservation = await _target.UpdateReservationAsync(reservationToUpdate);

            Assert.Equal(newDate, updatedReservation.Von);
        }

        [Fact]
        public async Task UpdateReservationWithOptimisticConcurrencyTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 3 },
                Kunde = new KundeDto { Id = 4 }
            };
            var reservationAToUpdate = await _target.InsertReservationAsync(reservationToInsert);
            reservationAToUpdate.Von = DateTime.UtcNow.AddMinutes(1).ToTimestamp();
            reservationAToUpdate.Auto = new AutoDto { Id = 3 };
            reservationAToUpdate.Kunde = new KundeDto { Id = 4 };
            await _target.UpdateReservationAsync(reservationAToUpdate);

            var reservationBToUpdate = reservationAToUpdate;
            reservationBToUpdate.Von = DateTime.UtcNow.AddMinutes(2).ToTimestamp();
            reservationBToUpdate.Auto = new AutoDto { Id = 3 };
            reservationBToUpdate.Kunde = new KundeDto { Id = 4 };
            try
            {
                await _target.UpdateReservationAsync(reservationBToUpdate);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.Aborted, e.StatusCode);
            }
        }

        [Fact]
        public async Task InsertReservationWithInvalidDateRangeTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            var reservationToCheck = new ReservationDto
            {
                Von = DateTime.UtcNow.AddMinutes(5).ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 2 },
                Kunde = new KundeDto { Id = 4 }
            };
            await _target.InsertReservationAsync(reservationToInsert);

            try
            {
                await _target.InsertReservationAsync(reservationToCheck);
            } catch (RpcException e)
            {
                Assert.Equal(StatusCode.OutOfRange, e.StatusCode);
            }
        }

        [Fact]
        public async Task InsertReservationWithAutoNotAvailableTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            var reservationToCheck = new ReservationDto
            {
                Von = DateTime.UtcNow.AddHours(5).ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(6).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            await _target.InsertReservationAsync(reservationToInsert);

            try
            {
                await _target.InsertReservationAsync(reservationToCheck);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.ResourceExhausted, e.StatusCode);
            }
        }

        [Fact]
        public async Task UpdateReservationWithInvalidDateRangeTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            var reservationBToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.AddHours(5).ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(6).ToTimestamp(),
                Auto = new AutoDto { Id = 2 },
                Kunde = new KundeDto { Id = 4 }
            };
            await _target.InsertReservationAsync(reservationToInsert);
            var reservationToUpdate = await _target.InsertReservationAsync(reservationBToInsert);
            reservationToUpdate.Von = DateTime.UtcNow.ToTimestamp();
            reservationToUpdate.Bis = DateTime.UtcNow.AddMinutes(1).ToTimestamp();
            reservationToUpdate.Auto = new AutoDto { Id = 2 };
            reservationToUpdate.Kunde = new KundeDto { Id = 4 };
            try
            {
                await _target.UpdateReservationAsync(reservationToUpdate);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.OutOfRange, e.StatusCode);
            }
        }

        [Fact]
        public async Task UpdateReservationWithAutoNotAvailableTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            var reservationBToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.AddHours(5).ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(6).ToTimestamp(),
                Auto = new AutoDto { Id = 2 },
                Kunde = new KundeDto { Id = 4 }
            };
            await _target.InsertReservationAsync(reservationToInsert);
            var reservationToUpdate = await _target.InsertReservationAsync(reservationBToInsert);
            reservationToUpdate.Auto = new AutoDto { Id = 1 };
            reservationToUpdate.Kunde = new KundeDto { Id = 4 };
            try
            {
                await _target.UpdateReservationAsync(reservationToUpdate);
            }
            catch (RpcException e)
            {
                Assert.Equal(StatusCode.OutOfRange, e.StatusCode);
            }
        }

        [Fact]
        public async Task CheckAvailabilityIsTrueTest()
        {
            var reservationToCheck = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 3 },
                Kunde = new KundeDto { Id = 4 }
            };
            var result = await _target.CheckAvailabilityAsync(reservationToCheck);
            Assert.True(result.IsAvailable);
        }

        [Fact]
        public async Task CheckAvailabilityIsFalseTest()
        {
            var reservationToInsert = new ReservationDto
            {
                Von = DateTime.UtcNow.ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            var reservationToCheck = new ReservationDto
            {
                Von = DateTime.UtcNow.AddMinutes(5).ToTimestamp(),
                Bis = DateTime.UtcNow.AddHours(1).ToTimestamp(),
                Auto = new AutoDto { Id = 1 },
                Kunde = new KundeDto { Id = 4 }
            };
            await _target.InsertReservationAsync(reservationToInsert);

            var result = await _target.CheckAvailabilityAsync(reservationToCheck);
            Assert.False(result.IsAvailable);
        }
    }
}