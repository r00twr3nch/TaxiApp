using Microsoft.EntityFrameworkCore;
using TaxiApp.Core; 
namespace TaxiApp.Data.DbContext
{
    public class TaxiAppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public TaxiAppDbContext(DbContextOptions<TaxiAppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<TaxiType> TaxiTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
