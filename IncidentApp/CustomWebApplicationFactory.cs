using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WebApplication2.model;

namespace AppTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices((context, services) =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<IncidentsDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Get connection string from configuration
                var configuration = context.Configuration;
                var connectionString = configuration.GetConnectionString("IncidentsConnection");

                // Add DbContext with test database
                services.AddDbContext<IncidentsDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Build service provider
                var sp = services.BuildServiceProvider();

                // Initialize database
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<IncidentsDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}