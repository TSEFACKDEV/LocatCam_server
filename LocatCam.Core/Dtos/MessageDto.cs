namespace LocatCam.Core.Dtos;

/// <summary>
/// Représentation d'un favori pour les réponses API.
/// </summary>
public class FavoriDto
{
    /// <summary>Identifiant unique du favori.</summary>
    public Guid Id { get; set; }

    /// <summary>Identifiant de l'utilisateur.</summary>
    public Guid UserId { get; set; }

    /// <summary>Identifiant de l'annonce.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Date d'ajout aux favoris.</summary>
    public DateTime DateAjout { get; set; }

    /// <summary>Détails de l'annonce favorisée.</summary>
    public AnnonceDto? Annonce { get; set; }
}

/// <summary>
/// Représentation d'un message pour les réponses API.
/// </summary>
public class MessageDto
{
    /// <summary>Identifiant unique du message.</summary>
    public Guid Id { get; set; }

    /// <summary>Identifiant de l'annonce concernée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Identifiant de l'expéditeur.</summary>
    public Guid ExpediteurId { get; set; }

    /// <summary>Identifiant du destinataire.</summary>
    public Guid DestinataireId { get; set; }

    /// <summary>Contenu du message.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Date d'envoi.</summary>
    public DateTime DateEnvoi { get; set; }

    /// <summary>Indique si lu par le destinataire.</summary>
    public bool Lu { get; set; }

    /// <summary>Nom de l'expéditeur (renseigné dans les listes de conversations).</summary>
    public string? ExpediteurNom { get; set; }

    /// <summary>Nom du destinataire (renseigné dans les listes de conversations).</summary>
    public string? DestinataireNom { get; set; }

    /// <summary>Titre de l'annonce concernée (renseigné dans les listes de conversations).</summary>
    public string? AnnonceTitre { get; set; }
}

/// <summary>
/// Données d'envoi d'un message.
/// </summary>
public class MessageCreateDto
{
    /// <summary>Identifiant de l'annonce concernée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Identifiant du destinataire (bailleur).</summary>
    public Guid DestinataireId { get; set; }

    /// <summary>Contenu du message.</summary>
    public string Contenu { get; set; } = string.Empty;
}

/// <summary>
/// Données de création d'un signalement.
/// </summary>
public class SignalementCreateDto
{
    /// <summary>Identifiant de l'annonce signalée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Raison du signalement.</summary>
    public string Raison { get; set; } = string.Empty;
}

/// <summary>
/// Représentation d'un signalement pour les réponses API.
/// </summary>
public class SignalementDto
{
    /// <summary>Identifiant unique.</summary>
    public Guid Id { get; set; }

    /// <summary>Identifiant de l'annonce signalée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Identifiant de l'utilisateur qui signale.</summary>
    public Guid UserId { get; set; }

    /// <summary>Raison du signalement.</summary>
    public string Raison { get; set; } = string.Empty;

    /// <summary>Date du signalement.</summary>
    public DateTime DateSignalement { get; set; }

    /// <summary>Indique si traité par un admin.</summary>
    public bool Traite { get; set; }
}
