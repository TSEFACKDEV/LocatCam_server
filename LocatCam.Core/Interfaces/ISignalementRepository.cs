namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Entities;

/// <summary>
/// Contrat spécifique pour l'accès aux données des signalements.
/// </summary>
public interface ISignalementRepository : IRepository<Signalement>
{
    /// <summary>Récupère les signalements non traités.</summary>
    Task<IEnumerable<Signalement>> GetUntreatedAsync(CancellationToken cancellationToken = default);
}
