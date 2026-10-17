namespace LocatCam.Core.Entities;

/// <summary>
/// Signalement d'une annonce par un utilisateur.
/// </summary>
public class Signalement : BaseEntity
{
    /// <summary>Identifiant de l'annonce signalée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Annonce signalée.</summary>
    public Annonce? Annonce { get; set; }

    /// <summary>Identifiant de l'utilisateur qui signale.</summary>
    public Guid UserId { get; set; }

    /// <summary>Utilisateur qui a signalé.</summary>
    public User? User { get; set; }

    /// <summary>Raison du signalement.</summary>
    public string Raison { get; set; } = string.Empty;

    /// <summary>Date du signalement.</summary>
    public DateTime DateSignalement { get; set; } = DateTime.UtcNow;

    /// <summary>Indique si le signalement a été traité par un administrateur.</summary>
    public bool Traite { get; set; }
}
