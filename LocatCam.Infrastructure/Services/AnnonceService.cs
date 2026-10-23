namespace LocatCam.Infrastructure.Services;

using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Enums;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;

/// <summary>
/// Service de gestion des annonces de logement.
/// </summary>
public class AnnonceService : IAnnonceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAnnoncePhotoRepository _annoncePhotoRepository;

    /// <summary>Constructeur recevant les dépendances.</summary>
    public AnnonceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _annoncePhotoRepository = unitOfWork.AnnoncePhotos;
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<AnnonceDto> Items, int TotalCount)> GetPagedAsync(
        decimal? prixMin, decimal? prixMax, string? quartier, string? ville,
        string? type, bool? estMeuble, bool? disponible, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        TypeAnnonce? typeEnum = null;
        if (!string.IsNullOrWhiteSpace(type))
            typeEnum = Enum.Parse<TypeAnnonce>(type);

        var (items, totalCount) = await _unitOfWork.Annonces.GetPagedAsync(
            prixMin, prixMax, quartier, ville, typeEnum, estMeuble, disponible, page, pageSize, cancellationToken);

        return (items.Select(MapToDto), totalCount);
    }

    /// <inheritdoc/>
    public async Task<AnnonceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var annonce = await _unitOfWork.Annonces.GetByIdAsync(id, cancellationToken);
        if (annonce == null) return null;

        // Charger les photos explicitement si pas déjà incluses
        var photos = await _annoncePhotoRepository.GetByAnnonceIdAsync(id, cancellationToken);
        foreach (var photo in photos)
        {
            if (!annonce.Photos.Any(p => p.Id == photo.Id))
                annonce.Photos.Add(photo);
        }

        return MapToDto(annonce);
    }

    /// <inheritdoc/>
    public async Task<AnnonceDto> CreateAsync(Guid userId, AnnonceCreateDto dto, CancellationToken cancellationToken = default)
    {
        var annonce = new Annonce
        {
            Titre = dto.Titre,
            Description = dto.Description,
            Prix = dto.Prix,
            Type = Enum.Parse<TypeAnnonce>(dto.Type),
            Quartier = dto.Quartier,
            Ville = dto.Ville,
            Adresse = dto.Adresse,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            EstMeuble = dto.EstMeuble,
            NombrePieces = dto.NombrePieces,
            Disponible = dto.Disponible,
            UserId = userId,
            Statut = StatutAnnonce.Publiee,
            EstVerifie = false,
            DateCreation = DateTime.UtcNow
        };

        await _unitOfWork.Annonces.AddAsync(annonce, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(annonce.Id, cancellationToken) ?? throw new InvalidOperationException("Erreur lors de la création de l'annonce.");
    }

    /// <inheritdoc/>
    public async Task<AnnonceDto?> UpdateAsync(Guid id, Guid userId, AnnonceUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var annonce = await _unitOfWork.Annonces.GetByIdAsync(id, cancellationToken);
        if (annonce == null) return null;
        if (annonce.UserId != userId) throw new UnauthorizedAccessException("Vous n'êtes pas le propriétaire de cette annonce.");

        if (!string.IsNullOrWhiteSpace(dto.Titre)) annonce.Titre = dto.Titre;
        if (!string.IsNullOrWhiteSpace(dto.Description)) annonce.Description = dto.Description;
        if (dto.Prix.HasValue) annonce.Prix = dto.Prix.Value;
        if (!string.IsNullOrWhiteSpace(dto.Type)) annonce.Type = Enum.Parse<TypeAnnonce>(dto.Type);
        if (!string.IsNullOrWhiteSpace(dto.Quartier)) annonce.Quartier = dto.Quartier;
        if (!string.IsNullOrWhiteSpace(dto.Ville)) annonce.Ville = dto.Ville;
        if (!string.IsNullOrWhiteSpace(dto.Adresse)) annonce.Adresse = dto.Adresse;
        if (dto.Latitude.HasValue) annonce.Latitude = dto.Latitude.Value;
        if (dto.Longitude.HasValue) annonce.Longitude = dto.Longitude.Value;
        if (dto.EstMeuble.HasValue) annonce.EstMeuble = dto.EstMeuble.Value;
        if (dto.NombrePieces.HasValue) annonce.NombrePieces = dto.NombrePieces.Value;
        if (dto.Disponible.HasValue) annonce.Disponible = dto.Disponible.Value;

        annonce.DateMiseAJour = DateTime.UtcNow;

        await _unitOfWork.Annonces.UpdateAsync(annonce, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var annonce = await _unitOfWork.Annonces.GetByIdAsync(id, cancellationToken);
        if (annonce == null) return false;
        if (annonce.UserId != userId) throw new UnauthorizedAccessException("Vous n'êtes pas le propriétaire de cette annonce.");

        annonce.Statut = StatutAnnonce.Archivee;
        annonce.Disponible = false;
        annonce.DateMiseAJour = DateTime.UtcNow;

        await _unitOfWork.Annonces.UpdateAsync(annonce, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task AddPhotosAsync(Guid annonceId, IEnumerable<PhotoCreateDto> photos, CancellationToken cancellationToken = default)
    {
        var annonce = await _unitOfWork.Annonces.GetByIdAsync(annonceId, cancellationToken);
        if (annonce == null)
            throw new InvalidOperationException("Annonce non trouvée.");

        var ordre = (annonce.Photos?.Count ?? 0);
        foreach (var photo in photos)
        {
            var entity = new AnnoncePhoto
            {
                Id = Guid.NewGuid(),
                AnnonceId = annonceId,
                UrlCloudinary = photo.UrlCloudinary,
                PublicId = photo.PublicId,
                Ordre = photo.Ordre > 0 ? photo.Ordre : ordre++,
                EstPrincipale = photo.EstPrincipale,
                DateCreation = DateTime.UtcNow
            };
            await _unitOfWork.AnnoncePhotos.AddAsync(entity, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AnnonceDto>> GetByBoundingBoxAsync(
        double minLat, double maxLat, double minLng, double maxLng,
        CancellationToken cancellationToken = default)
    {
        var annonces = await _unitOfWork.Annonces.GetByBoundingBoxAsync(minLat, maxLat, minLng, maxLng, cancellationToken);
        return annonces.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AnnonceDto>> GetMesAnnoncesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var annonces = await _unitOfWork.Annonces.GetByUserIdAsync(userId, cancellationToken);
        return annonces.Select(MapToDto);
    }

    private static AnnonceDto MapToDto(Annonce a)
    {
        return new AnnonceDto
        {
            Id = a.Id,
            Titre = a.Titre,
            Description = a.Description,
            Prix = a.Prix,
            Type = a.Type.ToString(),
            Quartier = a.Quartier,
            Ville = a.Ville,
            Adresse = a.Adresse,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            EstMeuble = a.EstMeuble,
            NombrePieces = a.NombrePieces,
            Disponible = a.Disponible,
            UserId = a.UserId,
            NomBailleur = a.User?.Nom,
            TelephoneBailleur = a.User?.Telephone,
            EstVerifie = a.EstVerifie,
            Statut = a.Statut.ToString(),
            DateCreation = a.DateCreation,
            Photos = a.Photos.Select(p => new AnnoncePhotoDto
            {
                Id = p.Id,
                UrlCloudinary = p.UrlCloudinary,
                PublicId = p.PublicId,
                Ordre = p.Ordre,
                EstPrincipale = p.EstPrincipale
            }).ToList()
        };
    }
}
