namespace LocatCam.Core.Entities;

/// <summary>
/// Classe de base pour toutes les entités du domaine.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Identifiant unique de l'entité.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Date de création de l'enregistrement.</summary>
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    /// <summary>Date de dernière mise à jour de l'enregistrement.</summary>
    public DateTime? DateMiseAJour { get; set; }
}
