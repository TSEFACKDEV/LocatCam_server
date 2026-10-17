namespace LocatCam.Core.Interfaces;

/// <summary>
/// Contrat générique pour l'accès aux données d'une entité.
/// </summary>
/// <typeparam name="T">Type d'entité.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Récupère une entité par son identifiant.</summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Récupère toutes les entités.</summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Ajoute une entité.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Met à jour une entité.</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Supprime une entité (soft delete ou hard delete selon l'implémentation).</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Sauvegarde les modifications.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
