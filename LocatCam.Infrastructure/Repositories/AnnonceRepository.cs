namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Entities;
using LocatCam.Infrastructure.Data;
using LocatCam.Core.Enums;
using LocatCam.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implémentation du repository des annonces.
/// </summary>
public class AnnonceRepository : Repository<Annonce>, IAnnonceRepository
{
    public AnnonceRepository(LocatCamDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<(IEnumerable<Annonce> Items, int TotalCount)> GetPagedAsync(
        decimal? prixMin = null,
        decimal? prixMax = null,
        string? quartier = null,
        string? ville = null,
        TypeAnnonce? type = null,
        bool? estMeuble = null,
        bool? disponible = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Photos)
            .Where(a => a.Statut == StatutAnnonce.Publiee && a.Disponible);

        if (prixMin.HasValue)
            query = query.Where(a => a.Prix >= prixMin.Value);
        if (prixMax.HasValue)
            query = query.Where(a => a.Prix <= prixMax.Value);
        if (!string.IsNullOrWhiteSpace(quartier))
            query = query.Where(a => a.Quartier.Contains(quartier));
        if (!string.IsNullOrWhiteSpace(ville))
            query = query.Where(a => a.Ville.Contains(ville));
        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);
        if (estMeuble.HasValue)
            query = query.Where(a => a.EstMeuble == estMeuble.Value);
        if (disponible.HasValue)
            query = query.Where(a => a.Disponible == disponible.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.DateCreation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Annonce>> GetByBoundingBoxAsync(
        double minLat, double maxLat, double minLng, double maxLng,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Photos)
            .Where(a => a.Statut == StatutAnnonce.Publiee && a.Disponible)
            .Where(a => a.Latitude >= minLat && a.Latitude <= maxLat)
            .Where(a => a.Longitude >= minLng && a.Longitude <= maxLng)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Annonce>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Photos)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DateCreation)
            .ToListAsync(cancellationToken);
    }
}
