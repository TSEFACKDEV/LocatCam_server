namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Entities;
using LocatCam.Infrastructure.Data;
using LocatCam.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implémentation du repository des favoris.
/// </summary>
public class FavoriRepository : Repository<Favori>, IFavoriRepository
{
    public FavoriRepository(LocatCamDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Guid userId, Guid annonceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(f => f.UserId == userId && f.AnnonceId == annonceId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Favori>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Annonce)
            .ThenInclude(a => a.Photos)
            .Include(f => f.Annonce)
            .ThenInclude(a => a.User)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.DateAjout)
            .ToListAsync(cancellationToken);
    }
}
