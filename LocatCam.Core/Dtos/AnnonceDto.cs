namespace LocatCam.Core.Dtos;

/// <summary>
/// Données de création d'une annonce (bailleur uniquement).
/// </summary>
public class AnnonceCreateDto
{
    /// <summary>Titre accrocheur de l'annonce.</summary>
    public string Titre { get; set; } = string.Empty;

    /// <summary>Description détaillée du logement.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Prix mensuel en FCFA (XAF).</summary>
    public decimal Prix { get; set; }

    /// <summary>Type de logement.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Quartier où se situe le logement.</summary>
    public string Quartier { get; set; } = string.Empty;

    /// <summary>Ville du logement.</summary>
    public string Ville { get; set; } = string.Empty;

    /// <summary>Adresse complète (optionnelle).</summary>
    public string Adresse { get; set; } = string.Empty;

    /// <summary>Latitude du logement.</summary>
    public double Latitude { get; set; }

    /// <summary>Longitude du logement.</summary>
    public double Longitude { get; set; }

    /// <summary>Indique si le logement est meublé.</summary>
    public bool EstMeuble { get; set; }

    /// <summary>Nombre de pièces du logement.</summary>
    public int NombrePieces { get; set; }

    /// <summary>Indique si l'annonce est disponible.</summary>
    public bool Disponible { get; set; } = true;
}

/// <summary>
/// Données de mise à jour d'une annonce (propriétaire uniquement).
/// </summary>
public class AnnonceUpdateDto
{
    /// <summary>Titre accrocheur de l'annonce.</summary>
    public string? Titre { get; set; }

    /// <summary>Description détaillée du logement.</summary>
    public string? Description { get; set; }

    /// <summary>Prix mensuel en FCFA (XAF).</summary>
    public decimal? Prix { get; set; }

    /// <summary>Type de logement.</summary>
    public string? Type { get; set; }

    /// <summary>Quartier où se situe le logement.</summary>
    public string? Quartier { get; set; }

    /// <summary>Ville du logement.</summary>
    public string? Ville { get; set; }

    /// <summary>Adresse complète.</summary>
    public string? Adresse { get; set; }

    /// <summary>Latitude du logement.</summary>
    public double? Latitude { get; set; }

    /// <summary>Longitude du logement.</summary>
    public double? Longitude { get; set; }

    /// <summary>Indique si le logement est meublé.</summary>
    public bool? EstMeuble { get; set; }

    /// <summary>Nombre de pièces du logement.</summary>
    public int? NombrePieces { get; set; }

    /// <summary>Indique si l'annonce est disponible.</summary>
    public bool? Disponible { get; set; }
}

/// <summary>
/// Représentation d'une photo d'annonce pour les réponses API.
/// </summary>
public class AnnoncePhotoDto
{
    /// <summary>Identifiant unique de la photo.</summary>
    public Guid Id { get; set; }

    /// <summary>URL de l'image sur Cloudinary.</summary>
    public string UrlCloudinary { get; set; } = string.Empty;

    /// <summary>Identifiant public sur Cloudinary.</summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary>Ordre d'affichage.</summary>
    public int Ordre { get; set; }

    /// <summary>Indique si c'est la photo principale.</summary>
    public bool EstPrincipale { get; set; }
}

/// <summary>
/// Représentation complète d'une annonce pour les réponses API.
/// </summary>
public class AnnonceDto
{
    /// <summary>Identifiant unique.</summary>
    public Guid Id { get; set; }

    /// <summary>Titre accrocheur.</summary>
    public string Titre { get; set; } = string.Empty;

    /// <summary>Description détaillée.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Prix mensuel en FCFA.</summary>
    public decimal Prix { get; set; }

    /// <summary>Type de logement.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Quartier.</summary>
    public string Quartier { get; set; } = string.Empty;

    /// <summary>Ville.</summary>
    public string Ville { get; set; } = string.Empty;

    /// <summary>Adresse.</summary>
    public string Adresse { get; set; } = string.Empty;

    /// <summary>Latitude.</summary>
    public double Latitude { get; set; }

    /// <summary>Longitude.</summary>
    public double Longitude { get; set; }

    /// <summary>Indique si meublé.</summary>
    public bool EstMeuble { get; set; }

    /// <summary>Nombre de pièces.</summary>
    public int NombrePieces { get; set; }

    /// <summary>Indique si disponible.</summary>
    public bool Disponible { get; set; }

    /// <summary>Identifiant du bailleur.</summary>
    public Guid UserId { get; set; }

    /// <summary>Nom du bailleur.</summary>
    public string? NomBailleur { get; set; }

    /// <summary>Téléphone du bailleur (format camerounais).</summary>
    public string? TelephoneBailleur { get; set; }

    /// <summary>Indique si vérifiée par un admin.</summary>
    public bool EstVerifie { get; set; }

    /// <summary>Statut de publication.</summary>
    public string Statut { get; set; } = string.Empty;

    /// <summary>Date de création.</summary>
    public DateTime DateCreation { get; set; }

    /// <summary>Photos associées.</summary>
    public ICollection<AnnoncePhotoDto> Photos { get; set; } = new List<AnnoncePhotoDto>();
}

/// <summary>
/// Signature Cloudinary pour l'upload direct depuis le mobile.
/// </summary>
public class PhotoSignatureDto
{
    /// <summary>Horodatage de la signature.</summary>
    public long Timestamp { get; set; }

    /// <summary>Dossier de destination sur Cloudinary.</summary>
    public string Folder { get; set; } = "locatcam/annonces";

    /// <summary>Nom du preset d'upload.</summary>
    public string UploadPreset { get; set; } = string.Empty;

    /// <summary>Signature sécurisée générée côté serveur.</summary>
    public string Signature { get; set; } = string.Empty;
}
