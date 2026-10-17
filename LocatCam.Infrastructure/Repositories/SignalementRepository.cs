namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Entities;
using LocatCam.Infrastructure.Data;
using LocatCam.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implémentation du repository des signalements.
/// </summary>
public class SignalementRepository : Repository<Signalement>, ISignalementRepository
{
    public SignalementRepository(LocatCamDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IEnumerable<Signalement>> GetUntreatedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Annonce)
            .Include(s => s.User)
            .Where(s => !s.Traite)
            .OrderByDescending(s => s.DateSignalement)
            .ToListAsync(cancellationToken);
    }
}
