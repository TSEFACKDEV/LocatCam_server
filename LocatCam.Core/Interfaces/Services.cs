namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;

/// <summary>
/// Contrat du service d'authentification et de gestion des utilisateurs.
/// </summary>
public interface IAuthService
{
    /// <summary>Enregistre un nouvel utilisateur.</summary>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default);

    /// <summary>Connecte un utilisateur et retourne les tokens.</summary>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);

    /// <summary>Renouvelle le jeton d'accès via un refresh token.</summary>
    Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>Récupère les informations de l'utilisateur connecté.</summary>
    Task<UserDto?> GetMeAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat du service de gestion des annonces.
/// </summary>
public interface IAnnonceService
{
    /// <summary>Récupère les annonces paginées avec filtres.</summary>
    Task<(IEnumerable<AnnonceDto> Items, int TotalCount)> GetPagedAsync(
        decimal? prixMin, decimal? prixMax, string? quartier, string? ville,
        string? type, bool? estMeuble, bool? disponible, int page, int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Récupère une annonce par son identifiant avec ses photos.</summary>
    Task<AnnonceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Crée une annonce (bailleur uniquement).</summary>
    Task<AnnonceDto> CreateAsync(Guid userId, AnnonceCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Met à jour une annonce (propriétaire uniquement).</summary>
    Task<AnnonceDto?> UpdateAsync(Guid id, Guid userId, AnnonceUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Supprime (soft delete) une annonce (propriétaire uniquement).</summary>
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Déclare des photos Cloudinary liées à une annonce (après upload mobile).</summary>
    Task AddPhotosAsync(Guid annonceId, IEnumerable<PhotoCreateDto> photos, CancellationToken cancellationToken = default);

    /// <summary>Récupère les annonces dans une bounding box pour la carte.</summary>
    Task<IEnumerable<AnnonceDto>> GetByBoundingBoxAsync(
        double minLat, double maxLat, double minLng, double maxLng,
        CancellationToken cancellationToken = default);

    /// <summary>Récupère les annonces d'un bailleur connecté.</summary>
    Task<IEnumerable<AnnonceDto>> GetMesAnnoncesAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat du service de génération de signature Cloudinary.
/// </summary>
public interface IPhotoService
{
    /// <summary>Génère une signature sécurisée pour l'upload direct sur Cloudinary.</summary>
    Task<PhotoSignatureDto> GenerateSignatureAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat du service de gestion des favoris.
/// </summary>
public interface IFavoriService
{
    /// <summary>Récupère les favoris d'un utilisateur.</summary>
    Task<IEnumerable<FavoriDto>> GetFavorisAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Ajoute une annonce aux favoris.</summary>
    Task<bool> AddAsync(Guid userId, Guid annonceId, CancellationToken cancellationToken = default);

    /// <summary>Retire une annonce des favoris.</summary>
    Task<bool> RemoveAsync(Guid userId, Guid annonceId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat du service de messagerie entre utilisateurs.
/// </summary>
public interface IMessageService
{
    /// <summary>Récupère les conversations distinctes d'un utilisateur.</summary>
    Task<IEnumerable<MessageDto>> GetConversationsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Récupère la conversation entre deux utilisateurs.</summary>
    Task<IEnumerable<MessageDto>> GetConversationAsync(
        Guid user1Id, Guid user2Id, Guid? annonceId = null, CancellationToken cancellationToken = default);

    /// <summary>Envoie un message au bailleur d'une annonce.</summary>
    Task<MessageDto> SendAsync(Guid expediteurId, MessageCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Marque un message comme lu.</summary>
    Task<bool> MarkAsReadAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Marque comme lus tous les messages reçus d'un utilisateur.</summary>
    Task<bool> MarkConversationAsReadAsync(Guid userId, Guid autreUserId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat du service de signalement d'annonces.
/// </summary>
public interface ISignalementService
{
    /// <summary>Signale une annonce.</summary>
    Task<SignalementDto> CreateAsync(Guid userId, SignalementCreateDto dto, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat des services administratifs (vérification annonces, bannissement utilisateurs).
/// </summary>
public interface IAdminService
{
    /// <summary>Vérifie ou dévérifie une annonce.</summary>
    Task<bool> VerifierAnnonceAsync(Guid annonceId, bool verifier, CancellationToken cancellationToken = default);

    /// <summary>Bannit ou débannit un utilisateur.</summary>
    Task<bool> BannirUserAsync(Guid userId, bool bannir, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrat du service de génération et validation des tokens JWT.
/// </summary>
public interface ITokenService
{
    /// <summary>Génère un jeton d'accès JWT pour un utilisateur.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Génère un refresh token aléatoire.</summary>
    string GenerateRefreshToken();

    /// <summary>Valide un jeton d'accès et retourne l'identifiant utilisateur s'il est valide.</summary>
    Guid? ValidateAccessToken(string token);
}
