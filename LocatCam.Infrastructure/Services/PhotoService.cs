namespace LocatCam.Infrastructure.Services;

using System.Security.Cryptography;
using System.Text;
using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Service de génération de signature Cloudinary pour l'upload direct d'images.
/// </summary>
public class PhotoService : IPhotoService
{
    private readonly string _cloudName;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _uploadPreset;

    /// <summary>Constructeur recevant la configuration Cloudinary.</summary>
    public PhotoService(IConfiguration configuration)
    {
        _cloudName = configuration["Cloudinary:CloudName"] ?? "locatcam";
        _apiKey = configuration["Cloudinary:ApiKey"] ?? string.Empty;
        _apiSecret = configuration["Cloudinary:ApiSecret"] ?? string.Empty;
        _uploadPreset = configuration["Cloudinary:UploadPreset"] ?? "locatcam_unsigned";
    }

    /// <inheritdoc/>
    public async Task<PhotoSignatureDto> GenerateSignatureAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var folder = "locatcam/annonces";

        // Signature Cloudinary : SHA1 de "timestamp={timestamp}&folder={folder}&{api_secret}"
        var signatureString = $"timestamp={timestamp}&folder={folder}&{_apiSecret}";
        var signature = ComputeHash(signatureString);

        return new PhotoSignatureDto
        {
            Timestamp = timestamp,
            Folder = folder,
            UploadPreset = _uploadPreset,
            Signature = signature
        };
    }

    private static string ComputeHash(string input)
    {
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
