namespace LocatCam.Api.Controllers;

using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Contrôleur de génération de signature Cloudinary pour l'upload d'images.
/// </summary>
[ApiController]
[Route("api/photos")]
[Produces("application/json")]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;

    /// <summary>Constructeur recevant le service photo.</summary>
    public PhotosController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    /// <summary>
    /// Génère une signature sécurisée pour l'upload direct sur Cloudinary.
    /// Le mobile utilise cette signature pour uploader sans exposer les clés API.
    /// </summary>
    /// <param name="cancellationToken">Jeton d'annulation.</param>
    /// <returns>Signature Cloudinary (timestamp, folder, upload_preset, signature).</returns>
    /// <response code="200">Signature générée.</response>
    [HttpPost("signature")]
    [Authorize]
    public async Task<IActionResult> GenerateSignature(CancellationToken cancellationToken)
    {
        var signature = await _photoService.GenerateSignatureAsync(cancellationToken);
        return Ok(signature);
    }
}
