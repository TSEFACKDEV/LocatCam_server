namespace LocatCam.Core.Entities;

/// <summary>
/// Photo d'une annonce stockée sur Cloudinary.
/// </summary>
public class AnnoncePhoto : BaseEntity
{
    /// <summary>Identifiant de l'annonce concernée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Annonce concernée.</summary>
    public Annonce? Annonce { get; set; }

    /// <summary>URL de l'image optimisée sur Cloudinary.</summary>
    public string UrlCloudinary { get; set; } = string.Empty;

    /// <summary>Identifiant public de l'image sur Cloudinary.</summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary>Ordre d'affichage de la photo dans la galerie.</summary>
    public int Ordre { get; set; }

    /// <summary>Indique si c'est la photo principale de l'annonce.</summary>
    public bool EstPrincipale { get; set; }
}
