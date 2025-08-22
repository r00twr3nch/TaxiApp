using System.Threading.Tasks;

namespace TaxiApp.Data.Repositories
{
    public interface IRideRepository
    {
        Task<Ride> CreateRideAsync(Ride ride);
        Task<Ride> GetRideByIdAsync(int id);
    }
}
