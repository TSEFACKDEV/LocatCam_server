namespace LocatCam.Core.Entities;

using LocatCam.Core.Enums;

/// <summary>
/// Annonce de logement publiée par un bailleur.
/// </summary>
public class Annonce : BaseEntity
{
    /// <summary>Titre accrocheur de l'annonce.</summary>
    public string Titre { get; set; } = string.Empty;

    /// <summary>Description détaillée du logement.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Prix mensuel en FCFA (XAF).</summary>
    public decimal Prix { get; set; }

    /// <summary>Type de logement.</summary>
    public TypeAnnonce Type { get; set; }

    /// <summary>Quartier où se situe le logement.</summary>
    public string Quartier { get; set; } = string.Empty;

    /// <summary>Ville du logement.</summary>
    public string Ville { get; set; } = string.Empty;

    /// <summary>Adresse complète (optionnelle pour le détail).</summary>
    public string Adresse { get; set; } = string.Empty;

    /// <summary>Latitude du logement (coordonnées GPS).</summary>
    public double Latitude { get; set; }

    /// <summary>Longitude du logement (coordonnées GPS).</summary>
    public double Longitude { get; set; }

    /// <summary>Indique si le logement est meublé.</summary>
    public bool EstMeuble { get; set; }

    /// <summary>Nombre de pièces du logement.</summary>
    public int NombrePieces { get; set; }

    /// <summary>Indique si l'annonce est actuellement disponible à la location.</summary>
    public bool Disponible { get; set; } = true;

    /// <summary>Identifiant du bailleur propriétaire de l'annonce.</summary>
    public Guid UserId { get; set; }

    /// <summary>Bailleur propriétaire de l'annonce.</summary>
    public User? User { get; set; }

    /// <summary>Indique si l'annonce a été vérifiée par un administrateur.</summary>
    public bool EstVerifie { get; set; }

    /// <summary>Statut de publication de l'annonce.</summary>
    public StatutAnnonce Statut { get; set; } = StatutAnnonce.Brouillon;

    /// <summary>Photos associées à l'annonce (Cloudinary).</summary>
    public ICollection<AnnoncePhoto> Photos { get; set; } = new List<AnnoncePhoto>();

    /// <summary>Favoris liés à cette annonce.</summary>
    public ICollection<Favori> Favoris { get; set; } = new List<Favori>();

    /// <summary>Messages liés à cette annonce.</summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();

    /// <summary>Signalements liés à cette annonce.</summary>
    public ICollection<Signalement> Signalements { get; set; } = new List<Signalement>();
}
