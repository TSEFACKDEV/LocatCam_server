namespace LocatCam.Infrastructure.Repositories;

using LocatCam.Core.Entities;
using LocatCam.Infrastructure.Data;
using LocatCam.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implémentation générique du repository EF Core.
/// </summary>
/// <typeparam name="T">Type d'entité.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly LocatCamDbContext _context;
    protected readonly DbSet<T> _dbSet;

    /// <summary>Constructeur recevant le contexte de base de données.</summary>
    public Repository(LocatCamDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
