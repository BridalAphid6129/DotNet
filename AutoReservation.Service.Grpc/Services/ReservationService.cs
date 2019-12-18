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
    internal class ReservationService : Grpc.ReservationService.ReservationServiceBase
    {
        private readonly ILogger<ReservationService> _logger;
        private ReservationManager reservationenManager = new ReservationManager();
        public ReservationService(ILogger<ReservationService> logger)
        {
            _logger = logger;
        }

        public override async Task<ReservationDtoList> GetReservationen(Empty request, ServerCallContext context)
        {
            var response = new ReservationDtoList();
            response.Items.AddRange(await reservationenManager.GetAllReservationen().ConvertToDtos());
            return response;
        }

        public override async Task<ReservationDto> GetReservationenById(GetReservationRequest request, ServerCallContext context)
        {
            ReservationDto response = await reservationenManager.GetReservationById(request.Id).ConvertToDto();
            return response ?? throw new RpcException(new Status(StatusCode.NotFound, "ID is invalid."));
        }

        public override async Task<ReservationDto> InsertReservation(ReservationDto request, ServerCallContext context)
        {
            try { 
            var resrvation = request.ConvertToEntity();
            var response = await reservationenManager.AddReservation(resrvation);
            return response.ConvertToDto();
        } catch (Exception e)
            {
                if (e is InvalidDateRangeException)
                {
                    throw new RpcException(new Status(StatusCode.OutOfRange, e.Message));
                }

                if (e is AutoUnavailableException)
                {
                    throw new RpcException(new Status(StatusCode.ResourceExhausted, e.Message));
                }
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        public override async Task<ReservationDto> UpdateReservation(ReservationDto request, ServerCallContext context)
        {
            try
            {
                var reservation = request.ConvertToEntity();
                var response = await reservationenManager.UpdateReservation(reservation);
                return response.ConvertToDto();
            }
            catch (Exception e)
            {
                if (e is OptimisticConcurrencyException<Reservation> specificException)
                {
                    throw new RpcException(new Status(StatusCode.Aborted, e.Message), specificException.MergedEntity.ToString());
                }
                if (e is InvalidDateRangeException)
                {
                    throw new RpcException(new Status(StatusCode.OutOfRange, e.Message));
                }
                if (e is AutoUnavailableException)
                {
                    throw new RpcException(new Status(StatusCode.ResourceExhausted, e.Message));
                }
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }
        public override async Task<ReservationDto> DeleteReservation(ReservationDto request, ServerCallContext context)
        {
            var reservation = request.ConvertToEntity();
            var response = await reservationenManager.DeleteReservation(reservation);
            return response.ConvertToDto();
        }
        public override async Task<IsAvailableResponse> CheckAvailability(ReservationDto request, ServerCallContext context)
        {
            var reservation = request.ConvertToEntity();
            try
            {
                await reservationenManager.AvailabilityCheck(reservation);
            } catch (AutoUnavailableException)
            {
            return new IsAvailableResponse{ IsAvailable = false};

            }
            return new IsAvailableResponse{ IsAvailable = true };
        }
    }
}
