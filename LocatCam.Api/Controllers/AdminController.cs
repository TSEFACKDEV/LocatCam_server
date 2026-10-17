namespace LocatCam.Api.Controllers;

using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Contrôleur d'administration : vérification des annonces et bannissement des utilisateurs.
/// </summary>
[ApiController]
[Route("api/admin")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    /// <summary>Constructeur recevant le service admin.</summary>
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// Vérifie ou dévérifie une annonce (Admin uniquement).
    /// </summary>
    /// <param name="id">Identifiant de l'annonce.</param>
    /// <param name="verifier">true pour vérifier, false pour dévérifier.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    /// <response code="200">Annonce vérifiée/dévérifiée.</response>
    /// <response code="404">Annonce non trouvée.</response>
    /// <response code="403">Accès réservé aux administrateurs.</response>
    [HttpPut("annonces/{id}/verifier")]
    public async Task<IActionResult> VerifierAnnonce(Guid id, [FromQuery] bool verifier, CancellationToken cancellationToken)
    {
        var success = await _adminService.VerifierAnnonceAsync(id, verifier, cancellationToken);
        if (!success) return NotFound(new { status = 404, message = "Annonce non trouvée." });
        return Ok(new { status = 200, message = verifier ? "Annonce vérifiée." : "Annonce dévérifiée." });
    }

    /// <summary>
    /// Bannit ou débannit un utilisateur (Admin uniquement).
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur.</param>
    /// <param name="bannir">true pour bannir, false pour débannir.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat de l'opération.</returns>
    /// <response code="200">Utilisateur banni/débanni.</response>
    /// <response code="404">Utilisateur non trouvé.</response>
    /// <response code="403">Accès réservé aux administrateurs.</response>
    [HttpPut("users/{id}/bannir")]
    public async Task<IActionResult> BannirUser(Guid id, [FromQuery] bool bannir, CancellationToken cancellationToken)
    {
        var success = await _adminService.BannirUserAsync(id, bannir, cancellationToken);
        if (!success) return NotFound(new { status = 404, message = "Utilisateur non trouvé." });
        return Ok(new { status = 200, message = bannir ? "Utilisateur banni." : "Utilisateur débanni." });
    }
}
