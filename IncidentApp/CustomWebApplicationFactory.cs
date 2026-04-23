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
                // Supprimer l'ancien DbContext
                var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
               typeof(DbContextOptions<IncidentsDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Récupérer la config
                var configuration = context.Configuration;
                var connectionString =
               configuration.GetConnectionString("IncidentsConnection");
                // Ajouter le DbContext avec la bonne connexion
                services.AddDbContext<IncidentsDbContext>(options =>
                options.UseSqlServer(connectionString));
                // Construire le provider
                var sp = services.BuildServiceProvider();
                // Initialiser la BD
                using (var scope = sp.CreateScope())
                {
                    var db =
                   scope.ServiceProvider.GetRequiredService<IncidentsDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}