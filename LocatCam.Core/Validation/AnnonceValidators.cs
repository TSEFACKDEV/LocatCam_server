namespace LocatCam.Core.Validation;

using FluentValidation;
using LocatCam.Core.Dtos;

/// <summary>
/// Validateur pour l'inscription d'un utilisateur.
/// </summary>
public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(200).WithMessage("Le nom ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire.")
            .EmailAddress().WithMessage("L'email n'est pas valide.")
            .MaximumLength(255).WithMessage("L'email ne peut pas dépasser 255 caractères.");

        RuleFor(x => x.Telephone)
            .NotEmpty().WithMessage("Le téléphone est obligatoire.")
            .Matches(@"^\+237\s?[236]\d{7,8}$").WithMessage("Le téléphone doit être au format camerounais (+237 6XX XXX XXX).");

        RuleFor(x => x.MotDePasse)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
            .MinimumLength(6).WithMessage("Le mot de passe doit contenir au moins 6 caractères.")
            .MaximumLength(100).WithMessage("Le mot de passe ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Le rôle est obligatoire.")
            .Must(r => r == "Etudiant" || r == "Bailleur")
            .WithMessage("Le rôle doit être 'Etudiant' ou 'Bailleur'.");
    }
}

/// <summary>
/// Validateur pour la connexion d'un utilisateur.
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire.")
            .EmailAddress().WithMessage("L'email n'est pas valide.");

        RuleFor(x => x.MotDePasse)
            .NotEmpty().WithMessage("Le mot de passe est obligatoire.");
    }
}

/// <summary>
/// Validateur pour la création d'une annonce.
/// </summary>
public class AnnonceCreateDtoValidator : AbstractValidator<AnnonceCreateDto>
{
    private static readonly string[] TypesValides = { "Chambre", "Studio", "Appartement", "Maison" };

    public AnnonceCreateDtoValidator()
    {
        RuleFor(x => x.Titre)
            .NotEmpty().WithMessage("Le titre est obligatoire.")
            .MaximumLength(200).WithMessage("Le titre ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La description est obligatoire.")
            .MaximumLength(2000).WithMessage("La description ne peut pas dépasser 2000 caractères.");

        RuleFor(x => x.Prix)
            .GreaterThan(0).WithMessage("Le prix doit être supérieur à 0 FCFA.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Le type de logement est obligatoire.")
            .Must(t => TypesValides.Contains(t))
            .WithMessage("Le type doit être Chambre, Studio, Appartement ou Maison.");

        RuleFor(x => x.Quartier)
            .NotEmpty().WithMessage("Le quartier est obligatoire.")
            .MaximumLength(100).WithMessage("Le quartier ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.Ville)
            .NotEmpty().WithMessage("La ville est obligatoire.")
            .MaximumLength(100).WithMessage("La ville ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.NombrePieces)
            .GreaterThan(0).WithMessage("Le nombre de pièces doit être supérieur à 0.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être comprise entre -90 et 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être comprise entre -180 et 180.");
    }
}

/// <summary>
/// Validateur pour la mise à jour d'une annonce.
/// </summary>
public class AnnonceUpdateDtoValidator : AbstractValidator<AnnonceUpdateDto>
{
    public AnnonceUpdateDtoValidator()
    {
        RuleFor(x => x.Titre)
            .MaximumLength(200).WithMessage("Le titre ne peut pas dépasser 200 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Titre));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("La description ne peut pas dépasser 2000 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Prix)
            .GreaterThan(0).WithMessage("Le prix doit être supérieur à 0 FCFA.")
            .When(x => x.Prix.HasValue);

        RuleFor(x => x.NombrePieces)
            .GreaterThan(0).WithMessage("Le nombre de pièces doit être supérieur à 0.")
            .When(x => x.NombrePieces.HasValue);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("La latitude doit être comprise entre -90 et 90.")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("La longitude doit être comprise entre -180 et 180.")
            .When(x => x.Longitude.HasValue);
    }
}

/// <summary>
/// Validateur pour l'envoi d'un message.
/// </summary>
public class MessageCreateDtoValidator : AbstractValidator<MessageCreateDto>
{
    public MessageCreateDtoValidator()
    {
        RuleFor(x => x.AnnonceId)
            .NotEmpty().WithMessage("L'identifiant de l'annonce est obligatoire.");

        RuleFor(x => x.DestinataireId)
            .NotEmpty().WithMessage("L'identifiant du destinataire est obligatoire.");

        RuleFor(x => x.Contenu)
            .NotEmpty().WithMessage("Le contenu du message est obligatoire.")
            .MaximumLength(2000).WithMessage("Le message ne peut pas dépasser 2000 caractères.");
    }
}

/// <summary>
/// Validateur pour la création d'un signalement.
/// </summary>
public class SignalementCreateDtoValidator : AbstractValidator<SignalementCreateDto>
{
    public SignalementCreateDtoValidator()
    {
        RuleFor(x => x.AnnonceId)
            .NotEmpty().WithMessage("L'identifiant de l'annonce est obligatoire.");

        RuleFor(x => x.Raison)
            .NotEmpty().WithMessage("La raison du signalement est obligatoire.")
            .MaximumLength(500).WithMessage("La raison ne peut pas dépasser 500 caractères.");
    }
}
