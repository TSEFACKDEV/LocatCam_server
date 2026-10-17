namespace LocatCam.Core.Dtos;

/// <summary>
/// Réponse paginée standard pour les listes d'éléments.
/// </summary>
/// <typeparam name="T">Type des éléments.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>Éléments de la page courante.</summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>Numéro de la page courante (à partir de 1).</summary>
    public int Page { get; set; }

    /// <summary>Taille de la page.</summary>
    public int PageSize { get; set; }

    /// <summary>Nombre total d'éléments.</summary>
    public int TotalCount { get; set; }

    /// <summary>Nombre total de pages.</summary>
    public int TotalPages { get; set; }
}

/// <summary>
/// Réponse d'erreur standardisée retournée par le middleware d'exception.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>Code HTTP du statut.</summary>
    public int Status { get; set; }

    /// <summary>Message d'erreur lisible.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Détails des erreurs de validation (champ => message).</summary>
    public IDictionary<string, string[]>? Errors { get; set; }
}
