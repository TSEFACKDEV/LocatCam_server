namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Entities;

/// <summary>
/// Contrat spécifique pour l'accès aux données des favoris.
/// </summary>
public interface IFavoriRepository : IRepository<Favori>
{
    /// <summary>Vérifie si une annonce est déjà en favoris pour un utilisateur.</summary>
    Task<bool> ExistsAsync(Guid userId, Guid annonceId, CancellationToken cancellationToken = default);

    /// <summary>Récupère les favoris d'un utilisateur avec les annonces.</summary>
    Task<IEnumerable<Favori>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
