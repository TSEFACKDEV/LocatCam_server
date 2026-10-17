namespace LocatCam.Api.Controllers;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Contrôleur de signalement d'annonces.
/// </summary>
[ApiController]
[Route("api/signalements")]
[Produces("application/json")]
public class SignalementsController : ControllerBase
{
    private readonly ISignalementService _signalementService;

    /// <summary>Constructeur recevant le service de signalement.</summary>
    public SignalementsController(ISignalementService signalementService)
    {
        _signalementService = signalementService;
    }

    /// <summary>
    /// Signale une annonce.
    /// </summary>
    /// <param name="dto">Identifiant de l'annonce et raison du signalement.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Signalement créé.</returns>
    /// <response code="201">Signalement créé.</response>
    /// <response code="400">Données invalides.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] SignalementCreateDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var signalement = await _signalementService.CreateAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = signalement.Id }, signalement);
    }
}
