namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Entities;
using LocatCam.Core.Enums;

/// <summary>
/// Contrat spécifique pour l'accès aux données des annonces.
/// </summary>
public interface IAnnonceRepository : IRepository<Annonce>
{
    /// <summary>
    /// Récupère les annonces paginées avec filtres de recherche.
    /// </summary>
    /// <param name="prixMin">Prix minimum en FCFA.</param>
    /// <param name="prixMax">Prix maximum en FCFA.</param>
    /// <param name="quartier">Filtre par quartier.</param>
    /// <param name="ville">Filtre par ville.</param>
    /// <param name="type">Filtre par type de logement.</param>
    /// <param name="estMeuble">Filtre par meublé.</param>
    /// <param name="disponible">Filtre par disponibilité.</param>
    /// <param name="page">Numéro de page (à partir de 1).</param>
    /// <param name="pageSize">Nombre d'éléments par page.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    Task<(IEnumerable<Annonce> Items, int TotalCount)> GetPagedAsync(
        decimal? prixMin = null,
        decimal? prixMax = null,
        string? quartier = null,
        string? ville = null,
        TypeAnnonce? type = null,
        bool? estMeuble = null,
        bool? disponible = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les annonces dans une bounding box (pour la carte).
    /// </summary>
    Task<IEnumerable<Annonce>> GetByBoundingBoxAsync(
        double minLat, double maxLat, double minLng, double maxLng,
        CancellationToken cancellationToken = default);

    /// <summary>Récupère les annonces d'un bailleur donné.</summary>
    Task<IEnumerable<Annonce>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
