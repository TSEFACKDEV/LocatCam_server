namespace LocatCam.Api.Controllers;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

/// <summary>
/// Contrôleur d'authentification : inscription, connexion, refresh, profil.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>Constructeur recevant le service d'authentification.</summary>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Inscrit un nouvel utilisateur (étudiant ou bailleur).
    /// </summary>
    /// <param name="dto">Données d'inscription.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Access token + refresh token + infos utilisateur.</returns>
    /// <response code="200">Inscription réussie.</response>
    /// <response code="400">Données invalides ou email/téléphone déjà utilisé.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { status = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// Connecte un utilisateur et retourne les tokens d'accès.
    /// </summary>
    /// <param name="dto">Identifiants de connexion.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Access token + refresh token + infos utilisateur.</returns>
    /// <response code="200">Connexion réussie.</response>
    /// <response code="401">Identifiants invalides.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { status = 401, message = ex.Message });
        }
    }

    /// <summary>
    /// Renouvelle le jeton d'accès via un refresh token.
    /// </summary>
    /// <param name="dto">Refresh token valide.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Nouvel access token + nouveau refresh token.</returns>
    /// <response code="200">Renouvellement réussi.</response>
    /// <response code="400">Refresh token invalide.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
            return BadRequest(new { status = 400, message = "Refresh token manquant." });

        try
        {
            var result = await _authService.RefreshAsync(dto.RefreshToken, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { status = 400, message = ex.Message });
        }
    }

    /// <summary>
    /// Récupère les informations de l'utilisateur actuellement connecté.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Détails de l'utilisateur connecté.</returns>
    /// <response code="200">Informations utilisateur.</response>
    /// <response code="401">Non authentifié.</response>
    /// <response code="404">Utilisateur non trouvé.</response>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var user = await _authService.GetMeAsync(userId, cancellationToken);
        if (user == null) return NotFound(new { status = 404, message = "Utilisateur non trouvé." });

        return Ok(user);
    }
}
