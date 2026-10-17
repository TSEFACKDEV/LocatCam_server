namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Entities;

/// <summary>
/// Contrat spécifique pour l'accès aux données des photos d'annonces.
/// </summary>
public interface IAnnoncePhotoRepository : IRepository<AnnoncePhoto>
{
    /// <summary>Récupère les photos d'une annonce ordonnées.</summary>
    Task<IEnumerable<AnnoncePhoto>> GetByAnnonceIdAsync(Guid annonceId, CancellationToken cancellationToken = default);
}
