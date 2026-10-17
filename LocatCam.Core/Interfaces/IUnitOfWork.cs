namespace LocatCam.Core.Interfaces;

/// <summary>
/// Contrat d'unité de travail pour coordonner plusieurs repositories et sauver en une fois.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IAnnonceRepository Annonces { get; }
    IAnnoncePhotoRepository AnnoncePhotos { get; }
    IFavoriRepository Favoris { get; }
    IMessageRepository Messages { get; }
    ISignalementRepository Signalements { get; }

    /// <summary>Sauvegarde toutes les modifications en attente.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
