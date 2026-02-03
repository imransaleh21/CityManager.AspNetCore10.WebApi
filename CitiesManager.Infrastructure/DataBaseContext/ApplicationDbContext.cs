using CitiesManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DataBaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
            
        }
        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().HasData( new City()
            {
                CityId = Guid.Parse("4D2648E5-4820-4DEA-B8DE-E606E243C535"),
                CityName = "Dhaka"
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                CityId = Guid.Parse("D25858F7-1566-47D8-80E5-40078B916140"),
                CityName = "Dinajpur"
            });
        }
    }
}
