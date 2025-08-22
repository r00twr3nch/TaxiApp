using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxiApp.Data.DbContext;

namespace TaxiApp.Data.Repositories
{
    public class TaxiTypeRepository : ITaxiTypeRepository
    {
        private readonly TaxiAppDbContext _context;

        public TaxiTypeRepository(TaxiAppDbContext context)
        {
            _context = context;
        }

        public async Task<TaxiType> GetByIdAsync(int id)
        {
            return await _context.TaxiTypes.FindAsync(id);
        }

        public async Task<IEnumerable<TaxiType>> GetAllAsync()
        {
            return await _context.TaxiTypes.ToListAsync();
        }
    }
}
