namespace LocatCam.Core.Enums;

/// <summary>
/// Rôles utilisateurs de l'application.
/// </summary>
public enum Role
{
    Etudiant,
    Bailleur,
    Admin
}

/// <summary>
/// Type de logement proposé dans une annonce.
/// </summary>
public enum TypeAnnonce
{
    Chambre,
    Studio,
    Appartement,
    Maison
}

/// <summary>
/// Statut de publication d'une annonce.
/// </summary>
public enum StatutAnnonce
{
    Brouillon,
    Publiee,
    Louee,
    Archivee
}
