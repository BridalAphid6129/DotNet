using AutoReservation.BusinessLayer;
using AutoReservation.BusinessLayer.Exceptions;
using AutoReservation.Dal.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AutoReservation.Service.Grpc.Services
{
    internal class AutoService : Grpc.AutoService.AutoServiceBase
    {
        private readonly ILogger<AutoService> _logger;
        private AutoManager autoManager = new AutoManager();

        public AutoService(ILogger<AutoService> logger)
        {
            _logger = logger;
        }

        public override async Task<AutoDtoList> GetAutos(Empty request, ServerCallContext context)
        {
                var response = new AutoDtoList();
                response.Items.AddRange(await autoManager.GetAllAutos().ConvertToDtos());
                return response;
        }

        public override async Task<AutoDto> GetAutoById(GetAutoRequest request, ServerCallContext context)
        {
            AutoDto response = await autoManager.GetAutoById(request.Id).ConvertToDto();
            return response ?? throw new RpcException(new Status(StatusCode.NotFound, "ID is invalid."));
        }

        public override async Task<AutoDto> InsertAuto(AutoDto request, ServerCallContext context)
        {
                var auto = request.ConvertToEntity();
                var response = await autoManager.AddAuto(auto);
                return response.ConvertToDto();
        }

        public override async Task<AutoDto> UpdateAuto(AutoDto request, ServerCallContext context)
        {
            try
            {
                var auto = request.ConvertToEntity();
                var response = await autoManager.ModifyAuto(auto);
                return response.ConvertToDto();
            }
            catch (OptimisticConcurrencyException<Auto> e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, e.Message), e.MergedEntity.ToString());
            }
        }

        public override async Task<AutoDto> DeleteAuto(AutoDto request, ServerCallContext context)
        {
                var auto = request.ConvertToEntity();
                var response = await autoManager.DeleteAuto(auto);
                return response.ConvertToDto();
        }
    }
}
