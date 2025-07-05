using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TourPlanner.DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TourPlannerContext>
    {
        public TourPlannerContext CreateDbContext(string[] args)
        {
            // Lade die Konfiguration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TourPlannerContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            return new TourPlannerContext(builder.Options);
        }
    }
}
