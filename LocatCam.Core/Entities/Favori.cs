namespace LocatCam.Core.Entities;

/// <summary>
/// Favori : un utilisateur marque une annonce comme favorite.
/// Contrainte d'unicité sur (UserId, AnnonceId).
/// </summary>
public class Favori : BaseEntity
{
    /// <summary>Identifiant de l'utilisateur qui ajoute aux favoris.</summary>
    public Guid UserId { get; set; }

    /// <summary>Utilisateur concerné.</summary>
    public User? User { get; set; }

    /// <summary>Identifiant de l'annonce ajoutée aux favoris.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Annonce concernée.</summary>
    public Annonce? Annonce { get; set; }

    /// <summary>Date d'ajout aux favoris.</summary>
    public DateTime DateAjout { get; set; } = DateTime.UtcNow;
}
