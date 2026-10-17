namespace LocatCam.Core.Interfaces;

using LocatCam.Core.Entities;

/// <summary>
/// Contrat spécifique pour l'accès aux données des utilisateurs.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Récupère un utilisateur par son email.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Vérifie si un email existe déjà.</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
