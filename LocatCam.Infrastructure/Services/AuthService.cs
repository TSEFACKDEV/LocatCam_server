namespace LocatCam.Infrastructure.Services;

using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using BCrypt = BCrypt.Net.BCrypt;
using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Enums;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Service d'authentification : inscription, connexion, refresh, profil.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    /// <summary>Constructeur recevant les dépendances.</summary>
    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        var emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email, cancellationToken);
        if (emailExists)
            throw new InvalidOperationException("Cet email est déjà utilisé.");

        var telephoneExists = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        if (telephoneExists.Any(u => u.Telephone == dto.Telephone))
            throw new InvalidOperationException("Ce numéro de téléphone est déjà utilisé.");

        var role = Enum.Parse<Role>(dto.Role);

        var user = new User
        {
            Nom = dto.Nom,
            Email = dto.Email,
            Telephone = dto.Telephone,
            MotDePasseHash = BCrypt.HashPassword(dto.MotDePasse),
            Role = role,
            EstVerifie = role == Role.Admin
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await BuildAuthResponseAsync(user, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("Email ou mot de passe incorrect.");

        if (!BCrypt.Verify(dto.MotDePasse, user.MotDePasseHash))
            throw new InvalidOperationException("Email ou mot de passe incorrect.");

        return await BuildAuthResponseAsync(user, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // En production, le refresh token serait stocké en base avec une date d'expiration.
        // Ici, on régénère simplement une nouvelle paire de tokens à partir du refresh token fourni.
        // Le refresh token sert d'identifiant opaque ; on retourne un nouveau access token.
        // Pour simplifier, on génère un nouvel access token pour un utilisateur par défaut.
        // Une implémentation complète stockerait les refresh tokens en base.
        throw new NotImplementedException("Le refresh token doit être associé à un utilisateur en base (à implémenter avec stockage des refresh tokens).");
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetMeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Nom = user.Nom,
            Email = user.Email,
            Telephone = user.Telephone,
            Role = user.Role.ToString(),
            PhotoUrl = user.PhotoUrl,
            EstVerifie = user.EstVerifie,
            DateCreation = user.DateCreation
        };
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"));

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration,
            User = new UserDto
            {
                Id = user.Id,
                Nom = user.Nom,
                Email = user.Email,
                Telephone = user.Telephone,
                Role = user.Role.ToString(),
                PhotoUrl = user.PhotoUrl,
                EstVerifie = user.EstVerifie,
                DateCreation = user.DateCreation
            }
        };
    }
}
