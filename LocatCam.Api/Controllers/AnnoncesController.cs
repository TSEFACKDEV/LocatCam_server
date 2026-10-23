namespace LocatCam.Api.Controllers;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

/// <summary>
/// Contrôleur de gestion des annonces de logement.
/// </summary>
[ApiController]
[Route("api/annonces")]
[Produces("application/json")]
public class AnnoncesController : ControllerBase
{
    private readonly IAnnonceService _annonceService;

    /// <summary>Constructeur recevant le service d'annonces.</summary>
    public AnnoncesController(IAnnonceService annonceService)
    {
        _annonceService = annonceService;
    }

    /// <summary>
    /// Récupère les annonces paginées avec filtres de recherche.
    /// </summary>
    /// <param name="prixMin">Prix minimum en FCFA.</param>
    /// <param name="prixMax">Prix maximum en FCFA.</param>
    /// <param name="quartier">Filtre par quartier.</param>
    /// <param name="ville">Filtre par ville.</param>
    /// <param name="type">Type de logement (Chambre, Studio, Appartement, Maison).</param>
    /// <param name="estMeuble">Filtre par meublé.</param>
    /// <param name="disponible">Filtre par disponibilité.</param>
    /// <param name="page">Numéro de page (à partir de 1).</param>
    /// <param name="pageSize">Nombre d'éléments par page.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste paginée d'annonces.</returns>
    /// <response code="200">Liste des annonces.</response>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaged(
        [FromQuery] decimal? prixMin,
        [FromQuery] decimal? prixMax,
        [FromQuery] string? quartier,
        [FromQuery] string? ville,
        [FromQuery] string? type,
        [FromQuery] bool? estMeuble,
        [FromQuery] bool? disponible,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _annonceService.GetPagedAsync(
            prixMin, prixMax, quartier, ville, type, estMeuble, disponible, page, pageSize, cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        return Ok(new
        {
            items,
            page,
            pageSize,
            totalCount,
            totalPages
        });
    }

    /// <summary>
    /// Récupère le détail d'une annonce avec ses photos.
    /// </summary>
    /// <param name="id">Identifiant de l'annonce.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Détail de l'annonce.</returns>
    /// <response code="200">Annonce trouvée.</response>
    /// <response code="404">Annonce non trouvée.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var annonce = await _annonceService.GetByIdAsync(id, cancellationToken);
        if (annonce == null) return NotFound(new { status = 404, message = "Annonce non trouvée." });
        return Ok(annonce);
    }

    /// <summary>
    /// Crée une nouvelle annonce (bailleur uniquement).
    /// </summary>
    /// <param name="dto">Données de l'annonce à créer.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Annonce créée.</returns>
    /// <response code="201">Annonce créée avec succès.</response>
    /// <response code="400">Données invalides.</response>
    /// <response code="401">Non authentifié.</response>
    /// <response code="403">Accès réservé aux bailleurs.</response>
    [HttpPost]
    [Authorize(Roles = "Bailleur")]
    public async Task<IActionResult> Create([FromBody] AnnonceCreateDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var annonce = await _annonceService.CreateAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = annonce.Id }, annonce);
    }

    /// <summary>
    /// Met à jour une annonce (propriétaire uniquement).
    /// </summary>
    /// <param name="id">Identifiant de l'annonce.</param>
    /// <param name="dto">Données de mise à jour.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Annonce mise à jour.</returns>
    /// <response code="200">Annonce mise à jour.</response>
    /// <response code="403">Vous n'êtes pas le propriétaire.</response>
    /// <response code="404">Annonce non trouvée.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Bailleur")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AnnonceUpdateDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        try
        {
            var annonce = await _annonceService.UpdateAsync(id, userId, dto, cancellationToken);
            if (annonce == null) return NotFound(new { status = 404, message = "Annonce non trouvée." });
            return Ok(annonce);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Supprime (soft delete) une annonce (propriétaire uniquement).
    /// </summary>
    /// <param name="id">Identifiant de l'annonce.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat de la suppression.</returns>
    /// <response code="204">Annonce supprimée.</response>
    /// <response code="403">Vous n'êtes pas le propriétaire.</response>
    /// <response code="404">Annonce non trouvée.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Bailleur")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        try
        {
            var success = await _annonceService.DeleteAsync(id, userId, cancellationToken);
            if (!success) return NotFound(new { status = 404, message = "Annonce non trouvée." });
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Récupère les annonces dans une bounding box (pour l'affichage carte).
    /// </summary>
    /// <param name="minLat">Latitude minimum.</param>
    /// <param name="maxLat">Latitude maximum.</param>
    /// <param name="minLng">Longitude minimum.</param>
    /// <param name="maxLng">Longitude maximum.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des annonces dans la zone.</returns>
    /// <response code="200">Annonces dans la bounding box.</response>
    [HttpGet("map")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByBoundingBox(
        [FromQuery] double minLat,
        [FromQuery] double maxLat,
        [FromQuery] double minLng,
        [FromQuery] double maxLng,
        CancellationToken cancellationToken)
    {
        var annonces = await _annonceService.GetByBoundingBoxAsync(minLat, maxLat, minLng, maxLng, cancellationToken);
        return Ok(annonces);
    }

    /// <summary>
    /// Récupère les annonces du bailleur actuellement connecté.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des annonces du bailleur.</returns>
    /// <response code="200">Annonces du bailleur.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpGet("mes-annonces")]
    [Authorize(Roles = "Bailleur")]
    public async Task<IActionResult> GetMesAnnonces(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var annonces = await _annonceService.GetMesAnnoncesAsync(userId, cancellationToken);
        return Ok(annonces);
    }

    /// <summary>
    /// Déclare des photos Cloudinary liées à une annonce (après upload direct côté mobile).
    /// </summary>
    /// <param name="id">Identifiant de l'annonce.</param>
    /// <param name="photos">Liste des photos à enregistrer.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <response code="200">Photos enregistrées.</response>
    /// <response code="401">Non authentifié.</response>
    /// <response code="403">Vous n'êtes pas le propriétaire.</response>
    /// <response code="404">Annonce non trouvée.</response>
    [HttpPost("{id}/photos")]
    [Authorize(Roles = "Bailleur")]
    public async Task<IActionResult> AddPhotos(
        Guid id,
        [FromBody] IEnumerable<PhotoCreateDto> photos,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var annonce = await _annonceService.GetByIdAsync(id, cancellationToken);
        if (annonce == null) return NotFound(new { status = 404, message = "Annonce non trouvée." });
        if (annonce.UserId != userId) return Forbid("Vous n'êtes pas le propriétaire de cette annonce.");

        var photoList = photos?.ToList() ?? new List<PhotoCreateDto>();
        await _annonceService.AddPhotosAsync(id, photoList, cancellationToken);
        return Ok(new { status = 200, message = $"{photoList.Count} photo(s) enregistrée(s)." });
    }
}
