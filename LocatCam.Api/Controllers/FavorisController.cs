namespace LocatCam.Api.Controllers;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Contrôleur de gestion des favoris.
/// </summary>
[ApiController]
[Route("api/favoris")]
[Produces("application/json")]
public class FavorisController : ControllerBase
{
    private readonly IFavoriService _favoriService;

    /// <summary>Constructeur recevant le service de favoris.</summary>
    public FavorisController(IFavoriService favoriService)
    {
        _favoriService = favoriService;
    }

    /// <summary>
    /// Récupère les favoris de l'utilisateur connecté.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des annonces favorisées.</returns>
    /// <response code="200">Favoris de l'utilisateur.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetFavoris(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var favoris = await _favoriService.GetFavorisAsync(userId, cancellationToken);
        return Ok(favoris);
    }

    /// <summary>
    /// Ajoute une annonce aux favoris de l'utilisateur connecté.
    /// </summary>
    /// <param name="annonceId">Identifiant de l'annonce.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat de l'ajout.</returns>
    /// <response code="200">Ajouté aux favoris.</response>
    /// <response code="400">Déjà dans les favoris.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpPost("{annonceId}")]
    [Authorize]
    public async Task<IActionResult> Add(Guid annonceId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var success = await _favoriService.AddAsync(userId, annonceId, cancellationToken);
        if (!success) return BadRequest(new { status = 400, message = "Cette annonce est déjà dans vos favoris." });
        return Ok(new { status = 200, message = "Annonce ajoutée aux favoris." });
    }

    /// <summary>
    /// Retire une annonce des favoris de l'utilisateur connecté.
    /// </summary>
    /// <param name="annonceId">Identifiant de l'annonce.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat de la suppression.</returns>
    /// <response code="200">Retiré des favoris.</response>
    /// <response code="404">Annonce non trouvée dans les favoris.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpDelete("{annonceId}")]
    [Authorize]
    public async Task<IActionResult> Remove(Guid annonceId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var success = await _favoriService.RemoveAsync(userId, annonceId, cancellationToken);
        if (!success) return NotFound(new { status = 404, message = "Annonce non trouvée dans vos favoris." });
        return Ok(new { status = 200, message = "Annonce retirée des favoris." });
    }
}
