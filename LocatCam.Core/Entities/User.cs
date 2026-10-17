namespace LocatCam.Core.Entities;

using LocatCam.Core.Enums;

/// <summary>
/// Utilisateur de l'application (étudiant, bailleur ou administrateur).
/// </summary>
public class User : BaseEntity
{
    /// <summary>Nom complet de l'utilisateur.</summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>Adresse email (identifiant de connexion).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Numéro de téléphone au format camerounais (+237 6XX XXX XXX).</summary>
    public string Telephone { get; set; } = string.Empty;

    /// <summary>Mot de passe hashé (BCrypt).</summary>
    public string MotDePasseHash { get; set; } = string.Empty;

    /// <summary>Rôle de l'utilisateur dans l'application.</summary>
    public Role Role { get; set; }

    /// <summary>URL de la photo de profil (Cloudinary).</summary>
    public string? PhotoUrl { get; set; }

    /// <summary>Indique si l'utilisateur a vérifié son compte.</summary>
    public bool EstVerifie { get; set; }

    /// <summary>Refresh token pour le renouvellement du jeton d'accès.</summary>
    public string? RefreshToken { get; set; }

    /// <summary>Date d'expiration du refresh token.</summary>
    public DateTime? RefreshTokenExpiry { get; set; }
}
