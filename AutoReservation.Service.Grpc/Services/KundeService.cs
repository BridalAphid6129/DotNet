using AutoReservation.BusinessLayer;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AutoReservation.Service.Grpc.Services
{
    internal class KundeService : Grpc.KundeService.KundeServiceBase
    {
        private readonly ILogger<KundeService> _logger;
        private KundeManager kundeManager = new KundeManager();

        public KundeService(ILogger<KundeService> logger)
        {
            _logger = logger;
        }

        public override async Task<KundeDtoList> GetKunden(Empty request, ServerCallContext context)
        {
            var response = new KundeDtoList();
            response.Items.AddRange(await kundeManager.GetAllKunden().ConvertToDtos());
            return response;
        }

        public override async Task<KundeDto> GetKunde(GetKundeRequest request, ServerCallContext context)
        {
            KundeDto response = await kundeManager.GetKundeById(request.IdFilter).ConvertToDto();
            return response ?? throw new RpcException(new Status(StatusCode.NotFound, "ID is invalid."));
        }

        public override async Task<KundeDto> InsertKunde(KundeDto request, ServerCallContext context)
        {
            var kunde = request.ConvertToEntity();
            var response = await kundeManager.AddKunde(kunde);
            return response.ConvertToDto();
        }

        public override async Task<KundeDto> UpdateKunde(KundeDto request, ServerCallContext context)
        {
            try
            {
                var kunde = request.ConvertToEntity();
                var response = await kundeManager.UpdateKunde(kunde);
                return response.ConvertToDto();
            }
            catch (OptimisticConcurrencyException<Kunde> e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, e.Message), e.MergedEntity.ToString());
            }
        }

        public override async Task<KundeDto> DeleteKunde(KundeDto request, ServerCallContext context)
        {
            var kunde = request.ConvertToEntity();
            var response = await kundeManager.DeleteKunde(kunde);
            return response.ConvertToDto();
        }
    }
}
