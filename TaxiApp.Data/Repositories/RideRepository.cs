using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxiApp.Data.DbContext;
namespace TaxiApp.Data.Repositories
{
    public class RideRepository : IRideRepository
    {
        private readonly TaxiAppDbContext _context;

        public RideRepository(TaxiAppDbContext context)
        {
            _context = context;
        }

        public async Task<Ride> CreateRideAsync(Ride ride)
        {
            _context.Rides.Add(ride);
            await _context.SaveChangesAsync();
            return ride;
        }

        public async Task<Ride> GetRideByIdAsync(int id)
        {
            return await _context.Rides.FindAsync(id);
        }
    }
}
