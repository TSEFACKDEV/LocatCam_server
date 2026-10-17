namespace LocatCam.Infrastructure.Services;

using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;

/// <summary>
/// Service de signalement d'annonces.
/// </summary>
public class SignalementService : ISignalementService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Constructeur recevant les dépendances.</summary>
    public SignalementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<SignalementDto> CreateAsync(Guid userId, SignalementCreateDto dto, CancellationToken cancellationToken = default)
    {
        var signalement = new Signalement
        {
            AnnonceId = dto.AnnonceId,
            UserId = userId,
            Raison = dto.Raison,
            DateSignalement = DateTime.UtcNow,
            Traite = false
        };

        await _unitOfWork.Signalements.AddAsync(signalement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SignalementDto
        {
            Id = signalement.Id,
            AnnonceId = signalement.AnnonceId,
            UserId = signalement.UserId,
            Raison = signalement.Raison,
            DateSignalement = signalement.DateSignalement,
            Traite = signalement.Traite
        };
    }
}
