namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Entities;

/// <summary>
/// Contrat spécifique pour l'accès aux données des messages.
/// </summary>
public interface IMessageRepository : IRepository<Message>
{
    /// <summary>Récupère les conversations distinctes d'un utilisateur.</summary>
    Task<IEnumerable<Message>> GetConversationsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Récupère la conversation entre deux utilisateurs (optionnellement sur une annonce).</summary>
    Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id, Guid? annonceId = null, CancellationToken cancellationToken = default);

    /// <summary>Marque un message comme lu.</summary>
    Task MarkAsReadAsync(Guid messageId, CancellationToken cancellationToken = default);
}
