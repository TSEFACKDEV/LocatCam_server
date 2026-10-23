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

        return await BuildAuthResponseAsync(user, cancellationToken, rotateRefreshToken: true);
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("Email ou mot de passe incorrect.");

        if (!BCrypt.Verify(dto.MotDePasse, user.MotDePasseHash))
            throw new InvalidOperationException("Email ou mot de passe incorrect.");

        return await BuildAuthResponseAsync(user, cancellationToken, rotateRefreshToken: true);
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Chercher l'utilisateur détenant ce refresh token et non expiré
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u =>
            u.RefreshToken == refreshToken &&
            u.RefreshTokenExpiry.HasValue &&
            u.RefreshTokenExpiry.Value > DateTime.UtcNow);

        if (user == null)
            throw new InvalidOperationException("Refresh token invalide ou expiré.");

        // Rotation : générer un nouveau refresh token
        return await BuildAuthResponseAsync(user, cancellationToken, rotateRefreshToken: true);
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

    /// <inheritdoc/>
    public async Task<UserDto?> UpdatePhotoAsync(Guid userId, string photoUrl, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null) return null;

        user.PhotoUrl = photoUrl;
        user.DateMiseAJour = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

    private async Task<AuthResponseDto> BuildAuthResponseAsync(User user, CancellationToken cancellationToken, bool rotateRefreshToken = false)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = rotateRefreshToken ? _tokenService.GenerateRefreshToken() : (user.RefreshToken ?? _tokenService.GenerateRefreshToken());
        var refreshExpiry = DateTime.UtcNow.AddDays(7);
        var accessExpiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"));

        // Mettre à jour le refresh token en base si rotation
        if (rotateRefreshToken)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = refreshExpiry;
            user.DateMiseAJour = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = accessExpiration,
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
