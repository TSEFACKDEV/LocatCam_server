namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Interfaces;
using LocatCam.Infrastructure.Data;

/// <summary>
/// Implémentation de l'unité de travail coordonnant tous les repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly LocatCamDbContext _context;

    /// <summary>Repository des utilisateurs.</summary>
    public IUserRepository Users { get; }

    /// <summary>Repository des annonces.</summary>
    public IAnnonceRepository Annonces { get; }

    /// <summary>Repository des photos d'annonces.</summary>
    public IAnnoncePhotoRepository AnnoncePhotos { get; }

    /// <summary>Repository des favoris.</summary>
    public IFavoriRepository Favoris { get; }

    /// <summary>Repository des messages.</summary>
    public IMessageRepository Messages { get; }

    /// <summary>Repository des signalements.</summary>
    public ISignalementRepository Signalements { get; }

    /// <summary>Constructeur recevant le contexte et initialisant les repositories.</summary>
    public UnitOfWork(LocatCamDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Annonces = new AnnonceRepository(context);
        AnnoncePhotos = new AnnoncePhotoRepository(context);
        Favoris = new FavoriRepository(context);
        Messages = new MessageRepository(context);
        Signalements = new SignalementRepository(context);
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
