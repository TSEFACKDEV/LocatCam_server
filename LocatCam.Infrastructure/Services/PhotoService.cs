namespace LocatCam.Infrastructure.Services;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LocatCam.Core.Dtos;
using LocatCam.Core.Interfaces;
using LocatCam.Core.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Service de génération de signature Cloudinary et upload d'images.
/// Utilise la bibliothèque CloudinaryDotNet pour la gestion des credentials et l'upload.
/// </summary>
public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _cloudName;
    private readonly string _apiKey;
    private readonly string _uploadPreset;
    private readonly string _apiSecret;

    /// <summary>Constructeur recevant la configuration Cloudinary.</summary>
    public PhotoService(IConfiguration configuration)
    {
        _cloudName = configuration["Cloudinary:CloudName"] ?? "locatcam";
        _apiKey = configuration["Cloudinary:ApiKey"] ?? string.Empty;
        _apiSecret = configuration["Cloudinary:ApiSecret"] ?? string.Empty;
        _uploadPreset = configuration["Cloudinary:UploadPreset"] ?? string.Empty;

        var account = new Account(_cloudName, _apiKey, _apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    /// <inheritdoc/>
    public Task<PhotoSignatureDto> GenerateSignatureAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var folder = "locatcam/annonces";

        // Cloudinary signe les paramètres triés par ordre alphabétique.
        // On ne signe que les paramètres réellement envoyés par le mobile :
        // un upload_preset absent côté Cloudinary ferait échouer l'upload.
        var parametres = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["folder"] = folder,
            ["timestamp"] = timestamp.ToString()
        };

        var preset = _uploadPreset.Trim();
        if (!string.IsNullOrEmpty(preset))
            parametres["upload_preset"] = preset;

        var signatureString = string.Join("&", parametres.Select(p => $"{p.Key}={p.Value}")) + _apiSecret;
        var signature = ComputeHash(signatureString);

        return Task.FromResult(new PhotoSignatureDto
        {
            CloudName = _cloudName,
            ApiKey = _apiKey,
            Timestamp = timestamp,
            Folder = folder,
            UploadPreset = preset,
            Signature = signature
        });
    }

    /// <summary>
    /// Upload une image sur Cloudinary (côté serveur, si nécessaire).
    /// </summary>
    /// <param name="filePath">Chemin local du fichier à uploader.</param>
    /// <param name="annonceId">Identifiant de l'annonce pour l'organisation du dossier.</param>
    /// <returns>Résultat de l'upload Cloudinary.</returns>
    public async Task<ImageUploadResult> UploadAsync(string filePath, Guid annonceId)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filePath),
            Folder = $"locatcam/annonces/{annonceId}",
            UseFilename = false,
            UniqueFilename = true,
            EagerTransforms = new List<Transformation>
            {
                new Transformation().Width(1200).Crop("limit").Quality("auto").FetchFormat("auto"), // Full
                new Transformation().Width(400).Crop("limit")                                       // Thumb
            }
        };

        return await _cloudinary.UploadAsync(uploadParams, cancellationToken: default);
    }

    private static string ComputeHash(string input)
    {
        using var sha1 = SHA1.Create();
        var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
