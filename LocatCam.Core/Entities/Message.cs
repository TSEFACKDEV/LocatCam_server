namespace LocatCam.Core.Entities;

/// <summary>
/// Message échangé entre un étudiant et un bailleur à propos d'une annonce.
/// </summary>
public class Message : BaseEntity
{
    /// <summary>Identifiant de l'annonce concernée.</summary>
    public Guid AnnonceId { get; set; }

    /// <summary>Annonce concernée.</summary>
    public Annonce? Annonce { get; set; }

    /// <summary>Identifiant de l'expéditeur du message.</summary>
    public Guid ExpediteurId { get; set; }

    /// <summary>Expéditeur du message.</summary>
    public User? Expediteur { get; set; }

    /// <summary>Identifiant du destinataire du message.</summary>
    public Guid DestinataireId { get; set; }

    /// <summary>Destinataire du message.</summary>
    public User? Destinataire { get; set; }

    /// <summary>Contenu textuel du message.</summary>
    public string Contenu { get; set; } = string.Empty;

    /// <summary>Date d'envoi du message.</summary>
    public DateTime DateEnvoi { get; set; } = DateTime.UtcNow;

    /// <summary>Indique si le message a été lu par le destinataire.</summary>
    public bool Lu { get; set; }
}
