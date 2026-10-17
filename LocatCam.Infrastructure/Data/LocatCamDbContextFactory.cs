namespace LocatCam.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Fabrique de contexte pour les outils EF Core en mode design-time (migrations).
/// </summary>
public class LocatCamDbContextFactory : IDesignTimeDbContextFactory<LocatCamDbContext>
{
    /// <summary>Crée une instance du DbContext pour les outils de conception.</summary>
    public LocatCamDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocatCamDbContext>();
        // Connection string par défaut pour le design-time (à surcharger via appsettings en runtime)
        optionsBuilder.UseNpgsql("Host=localhost;Database=locatcam;Username=postgres;Password=postgres;");
        return new LocatCamDbContext(optionsBuilder.Options);
    }
}
