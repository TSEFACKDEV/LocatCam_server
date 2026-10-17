namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Entities;
using LocatCam.Infrastructure.Data;
using LocatCam.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implémentation du repository des messages.
/// </summary>
public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(LocatCamDbContext context) : base(context) { }

    /// <inheritdoc/>
    public async Task<IEnumerable<Message>> GetConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Annonce)
            .Include(m => m.Expediteur)
            .Include(m => m.Destinataire)
            .Where(m => m.ExpediteurId == userId || m.DestinataireId == userId)
            .OrderByDescending(m => m.DateEnvoi)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Message>> GetConversationAsync(
        Guid user1Id, Guid user2Id, Guid? annonceId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.Annonce)
            .Include(m => m.Expediteur)
            .Include(m => m.Destinataire)
            .Where(m =>
                (m.ExpediteurId == user1Id && m.DestinataireId == user2Id) ||
                (m.ExpediteurId == user2Id && m.DestinataireId == user1Id));

        if (annonceId.HasValue)
            query = query.Where(m => m.AnnonceId == annonceId.Value);

        return await query.OrderBy(m => m.DateEnvoi).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task MarkAsReadAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _dbSet.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.Lu = true;
            _dbSet.Update(message);
        }
    }
}
