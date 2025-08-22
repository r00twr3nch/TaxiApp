using System;
using System.Threading.Tasks;
using TaxiApp.Data.Repositories;

namespace TaxiApp.Business.Services
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;
        private readonly ITaxiTypeRepository _taxiTypeRepository;
        private const decimal DefaultCommissionRate = 0.10m; // %10 komisyon

        public RideService(IRideRepository rideRepository, ITaxiTypeRepository taxiTypeRepository)
        {
            _rideRepository = rideRepository;
            _taxiTypeRepository = taxiTypeRepository;
        }

        public async Task<Ride> RequestRideAsync(RideRequestDto dto)
        {
            return await CreateRideInternal(dto);
        }

        public async Task<Ride> AddRide(RideRequestDto dto)
        {
            return await CreateRideInternal(dto);
        }

        private async Task<Ride> CreateRideInternal(RideRequestDto dto)
        {
            var taxiType = await _taxiTypeRepository.GetByIdAsync(dto.TaxiTypeId);
            if (taxiType == null)
                throw new Exception("Taxi tipi bulunamadı.");

            var totalPrice = CalculatePrice(dto.DistanceKm, taxiType.KmPrice, taxiType.BaseFare, DefaultCommissionRate);

            var ride = new Ride
            {
                PassengerId = dto.PassengerId,
                DriverId = dto.DriverId, 
                TaxiTypeId = dto.TaxiTypeId,
                StartLatitude = dto.StartLatitude,
                StartLongitude = dto.StartLongitude,
                EndLatitude = dto.EndLatitude,
                EndLongitude = dto.EndLongitude,
                DistanceKm = dto.DistanceKm,
                CommissionRate = DefaultCommissionRate,
                TotalPrice = totalPrice,
                Status = "Requested",
                RequestedAt = DateTime.UtcNow
            };
                
            return await _rideRepository.CreateRideAsync(ride);
        }

        public decimal CalculatePrice(decimal distanceKm, decimal kmPrice, decimal baseFare, decimal commissionRate)
        {
            var price = (distanceKm * kmPrice) + baseFare;
            var commission = price * commissionRate;
            return price + commission;
        }
    }
}
