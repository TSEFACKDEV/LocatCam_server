namespace LocatCam.Infrastructure.Services;

using LocatCam.Core.Entities;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;

/// <summary>
/// Service administratif : vérification des annonces et bannissement des utilisateurs.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Constructeur recevant les dépendances.</summary>
    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<bool> VerifierAnnonceAsync(Guid annonceId, bool verifier, CancellationToken cancellationToken = default)
    {
        var annonce = await _unitOfWork.Annonces.GetByIdAsync(annonceId, cancellationToken);
        if (annonce == null) return false;

        annonce.EstVerifie = verifier;
        annonce.DateMiseAJour = DateTime.UtcNow;

        await _unitOfWork.Annonces.UpdateAsync(annonce, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> BannirUserAsync(Guid userId, bool bannir, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;

        // On marque l'utilisateur comme non vérifié pour simuler un bannissement léger
        // (à affiner avec un champ EstBanni si nécessaire)
        user.EstVerifie = !bannir;
        user.DateMiseAJour = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
