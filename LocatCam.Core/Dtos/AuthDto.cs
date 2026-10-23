namespace LocatCam.Core.Dtos;

/// <summary>
/// Données d'inscription d'un utilisateur (étudiant ou bailleur).
/// </summary>
public class RegisterRequestDto
{
    /// <summary>Nom complet de l'utilisateur.</summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>Adresse email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Numéro de téléphone camerounais (+237 6XX XXX XXX).</summary>
    public string Telephone { get; set; } = string.Empty;

    /// <summary>Mot de passe en clair.</summary>
    public string MotDePasse { get; set; } = string.Empty;

    /// <summary>Rôle souhaité (Etudiant ou Bailleur).</summary>
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Données de connexion d'un utilisateur.
/// </summary>
public class LoginRequestDto
{
    /// <summary>Adresse email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Mot de passe en clair.</summary>
    public string MotDePasse { get; set; } = string.Empty;
}

/// <summary>
/// Requête de renouvellement de jeton d'accès (refresh token).
/// </summary>
public class RefreshRequestDto
{
    /// <summary>Refresh token valide précédemment émis.</summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Réponse d'authentification contenant les tokens et les informations utilisateur.
/// </summary>
public class AuthResponseDto
{
    /// <summary>Jeton d'accès JWT.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Jeton de renouvellement (refresh token).</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Date d'expiration du jeton d'accès.</summary>
    public DateTime Expiration { get; set; }

    /// <summary>Informations de l'utilisateur connecté.</summary>
    public UserDto User { get; set; } = new();
}

/// <summary>
/// Représentation simplifiée d'un utilisateur pour les réponses API.
/// </summary>
public class UserDto
{
    /// <summary>Identifiant unique.</summary>
    public Guid Id { get; set; }

    /// <summary>Nom complet.</summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>Adresse email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Numéro de téléphone.</summary>
    public string Telephone { get; set; } = string.Empty;

    /// <summary>Rôle de l'utilisateur.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>URL de la photo de profil.</summary>
    public string? PhotoUrl { get; set; }

    /// <summary>Indique si le compte est vérifié.</summary>
    public bool EstVerifie { get; set; }

    /// <summary>Date de création du compte.</summary>
    public DateTime DateCreation { get; set; }
}
