using System.Threading.Tasks;
using TaxiApp;

namespace TaxiApp.Data.Repositories
{
    public interface ITaxiTypeRepository
    {
        Task<TaxiType> GetByIdAsync(int id);
        Task<IEnumerable<TaxiType>> GetAllAsync();
    }
}
