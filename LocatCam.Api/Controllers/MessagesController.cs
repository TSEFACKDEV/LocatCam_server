namespace LocatCam.Api.Controllers;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Contrôleur de messagerie entre utilisateurs.
/// </summary>
[ApiController]
[Route("api/messages")]
[Produces("application/json")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    /// <summary>Constructeur recevant le service de messagerie.</summary>
    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Envoie un message au bailleur d'une annonce.
    /// </summary>
    /// <param name="dto">Contenu du message et destinataire.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Message envoyé.</returns>
    /// <response code="201">Message envoyé.</response>
    /// <response code="400">Données invalides.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Send([FromBody] MessageCreateDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var expediteurId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var message = await _messageService.SendAsync(expediteurId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetConversation), new { autreUserId = dto.DestinataireId }, message);
    }

    /// <summary>
    /// Récupère la liste des conversations distinctes de l'utilisateur connecté.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des conversations.</returns>
    /// <response code="200">Conversations de l'utilisateur.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpGet("conversations")]
    [Authorize]
    public async Task<IActionResult> GetConversations(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var conversations = await _messageService.GetConversationsAsync(userId, cancellationToken);
        return Ok(conversations);
    }

    /// <summary>
    /// Récupère la conversation entre l'utilisateur connecté et un autre utilisateur.
    /// </summary>
    /// <param name="autreUserId">Identifiant de l'autre utilisateur.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Liste des messages de la conversation.</returns>
    /// <response code="200">Messages de la conversation.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpGet("conversation/{autreUserId}")]
    [Authorize]
    public async Task<IActionResult> GetConversation(Guid autreUserId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var messages = await _messageService.GetConversationAsync(userId, autreUserId, cancellationToken: cancellationToken);
        return Ok(messages);
    }

    /// <summary>
    /// Marque un message comme lu.
    /// </summary>
    /// <param name="id">Identifiant du message.</param>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Résultat du marquage.</returns>
    /// <response code="200">Message marqué comme lu.</response>
    /// <response code="404">Message non trouvé.</response>
    /// <response code="401">Non authentifié.</response>
    [HttpPut("{id}/lu")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(new { status = 401, message = "Token invalide." });

        var success = await _messageService.MarkAsReadAsync(id, userId, cancellationToken);
        if (!success) return NotFound(new { status = 404, message = "Message non trouvé." });
        return Ok(new { status = 200, message = "Message marqué comme lu." });
    }
}
