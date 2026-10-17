namespace LocatCam.Infrastructure.Services;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;

/// <summary>
/// Service de gestion des favoris des utilisateurs.
/// </summary>
public class FavoriService : IFavoriService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Constructeur recevant les dépendances.</summary>
    public FavoriService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FavoriDto>> GetFavorisAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var favoris = await _unitOfWork.Favoris.GetByUserIdAsync(userId, cancellationToken);
        return favoris.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<bool> AddAsync(Guid userId, Guid annonceId, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.Favoris.ExistsAsync(userId, annonceId, cancellationToken);
        if (exists) return false;

        var favori = new LocatCam.Core.Entities.Favori
        {
            UserId = userId,
            AnnonceId = annonceId,
            DateAjout = DateTime.UtcNow
        };

        await _unitOfWork.Favoris.AddAsync(favori, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveAsync(Guid userId, Guid annonceId, CancellationToken cancellationToken = default)
    {
        var favoris = await _unitOfWork.Favoris.GetAllAsync(cancellationToken);
        var favori = favoris.FirstOrDefault(f => f.UserId == userId && f.AnnonceId == annonceId);
        if (favori == null) return false;

        await _unitOfWork.Favoris.DeleteAsync(favori, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static FavoriDto MapToDto(LocatCam.Core.Entities.Favori f)
    {
        return new FavoriDto
        {
            Id = f.Id,
            UserId = f.UserId,
            AnnonceId = f.AnnonceId,
            DateAjout = f.DateAjout,
            Annonce = f.Annonce != null ? new AnnonceDto
            {
                Id = f.Annonce.Id,
                Titre = f.Annonce.Titre,
                Prix = f.Annonce.Prix,
                Type = f.Annonce.Type.ToString(),
                Quartier = f.Annonce.Quartier,
                Ville = f.Annonce.Ville,
                NomBailleur = f.Annonce.User?.Nom,
                TelephoneBailleur = f.Annonce.User?.Telephone,
                EstVerifie = f.Annonce.EstVerifie,
                Photos = f.Annonce.Photos.Select(p => new AnnoncePhotoDto
                {
                    Id = p.Id,
                    UrlCloudinary = p.UrlCloudinary,
                    Ordre = p.Ordre,
                    EstPrincipale = p.EstPrincipale
                }).ToList()
            } : null
        };
    }
}
