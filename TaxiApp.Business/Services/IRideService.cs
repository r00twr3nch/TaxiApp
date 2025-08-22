using System.Threading.Tasks;
using TaxiApp;

namespace TaxiApp.Business.Services
{
    public interface IRideService
    {
        Task<Ride> RequestRideAsync(RideRequestDto dto);
        decimal CalculatePrice(decimal distanceKm, decimal kmPrice, decimal baseFare, decimal commissionRate);
        Task<Ride> AddRide(RideRequestDto dto);
    }
}
