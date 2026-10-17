namespace LocatCam.Infrastructure.Seed;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LocatCam.Infrastructure.Data;

/// <summary>
/// Extensions pour appliquer les migrations et exécuter le seed des données de test en environnement Development.
/// </summary>
public static class SeedExtensions
{
    /// <summary>
    /// Applique les migrations EF Core et exécute le seed des données de test en environnement Development.
    /// </summary>
    public static void ApplyMigrationsAndSeed(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<DataSeeder>>();

        try
        {
            var context = services.GetRequiredService<LocatCamDbContext>();
            context.Database.Migrate();

            var env = services.GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>();
            if (env.IsDevelopment())
            {
                var seeder = services.GetRequiredService<DataSeeder>();
                seeder.RunAsync().GetAwaiter().GetResult();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'application des migrations ou du seed.");
        }
    }
}
