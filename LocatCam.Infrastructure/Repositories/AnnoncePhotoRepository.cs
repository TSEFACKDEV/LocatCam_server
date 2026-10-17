namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Entities;
using LocatCam.Infrastructure.Data;
using LocatCam.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implémentation du repository des photos d'annonces.
/// </summary>
public class AnnoncePhotoRepository : Repository<AnnoncePhoto>, IAnnoncePhotoRepository
{
    public AnnoncePhotoRepository(LocatCamDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IEnumerable<AnnoncePhoto>> GetByAnnonceIdAsync(Guid annonceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.AnnonceId == annonceId)
            .OrderBy(p => p.Ordre)
            .ToListAsync(cancellationToken);
    }
}
