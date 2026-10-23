namespace LocatCam.Infrastructure.Services;

using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;

/// <summary>
/// Service de messagerie entre utilisateurs autour d'une annonce.
/// </summary>
public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Constructeur recevant les dépendances.</summary>
    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MessageDto>> GetConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.Messages.GetConversationsAsync(userId, cancellationToken);
        return messages.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MessageDto>> GetConversationAsync(
        Guid user1Id, Guid user2Id, Guid? annonceId = null, CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.Messages.GetConversationAsync(user1Id, user2Id, annonceId, cancellationToken);
        return messages.OrderBy(m => m.DateEnvoi).Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<MessageDto> SendAsync(Guid expediteurId, MessageCreateDto dto, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            AnnonceId = dto.AnnonceId,
            ExpediteurId = expediteurId,
            DestinataireId = dto.DestinataireId,
            Contenu = dto.Contenu,
            DateEnvoi = DateTime.UtcNow,
            Lu = false
        };

        await _unitOfWork.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(message);
    }

    /// <inheritdoc/>
    public async Task<bool> MarkAsReadAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Récupère le message pour vérifier que l'utilisateur est le destinataire
        var conversation = await _unitOfWork.Messages.GetConversationsAsync(userId, cancellationToken);
        var message = conversation.FirstOrDefault(m => m.Id == messageId);
        if (message == null) return false;

        await _unitOfWork.Messages.MarkAsReadAsync(messageId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MarkConversationAsReadAsync(Guid userId, Guid autreUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Messages.MarkConversationAsReadAsync(userId, autreUserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static MessageDto MapToDto(Message m)
    {
        return new MessageDto
        {
            Id = m.Id,
            AnnonceId = m.AnnonceId,
            ExpediteurId = m.ExpediteurId,
            DestinataireId = m.DestinataireId,
            Contenu = m.Contenu,
            DateEnvoi = m.DateEnvoi,
            Lu = m.Lu,
            ExpediteurNom = m.Expediteur?.Nom,
            DestinataireNom = m.Destinataire?.Nom,
            AnnonceTitre = m.Annonce?.Titre
        };
    }
}
