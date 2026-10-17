namespace LocatCam.Infrastructure.Seed;

using Bogus;
using BCrypt.Net;
using LocatCam.Core.Entities;
using LocatCam.Core.Enums;
using LocatCam.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Seed des données de test réalistes pour le Cameroun (Douala et Yaoundé).
/// Exécuté automatiquement en environnement Development uniquement.
/// </summary>
public class DataSeeder
{
    private readonly LocatCamDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    private static readonly string[] VillesDouala = { "Douala", "Douala", "Douala", "Douala", "Douala", "Douala", "Douala", "Douala", "Douala", "Douala" };
    private static readonly string[] VillesYaounde = { "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé", "Yaoundé" };

    // Quartiers réels de Douala avec coordonnées GPS approximatives
    private static readonly (string Quartier, double Lat, double Lng)[] QuartiersDouala =
    {
        ("Bonapriso", 4.0511, 9.7679),
        ("Bonanjo", 4.0543, 9.7703),
        ("Akwa", 4.0489, 9.7806),
        ("Bonabéri", 4.0356, 9.7333),
        ("Deido", 4.0437, 9.7683),
        ("Makepe", 4.0186, 9.7442),
        ("Logbessou", 4.0680, 9.7186),
        ("Ndogbong", 4.0620, 9.7766),
        ("Kotto", 4.0231, 9.7694),
        ("New-Bell", 4.0521, 9.7633)
    };

    // Quartiers réels de Yaoundé avec coordonnées GPS approximatives
    private static readonly (string Quartier, double Lat, double Lng)[] QuartiersYaounde =
    {
        ("Bastos", 3.8732, 11.5238),
        ("Mvog-Mbi", 3.8571, 11.5036),
        ("Ngoa-Ekelle", 3.8917, 11.5517),
        ("Biyem-Assi", 3.8231, 11.5031),
        ("Mendong", 3.8392, 11.5321),
        ("Odza", 3.8433, 11.5410),
        ("Nsam", 3.8833, 11.5467),
        ("Emana", 3.8778, 11.5222),
        ("Ekounou", 3.8620, 11.5310),
        ("Nkolbisson", 3.8292, 11.4878)
    };

    // Noms de famille camerounais réalistes
    private static readonly string[] NomsBailleurs =
    {
        "Mbarga", "Ngo Bell", "Fotso", "Tchoumi", "Eto'o", "Nkoulou",
        "Moumié", "Dika", "Biya", "Kamga", "Tchana", "Ndem"
    };

    private static readonly string[] NomsEtudiants =
    {
        "Kamdem", "Tchinda", "Njoumou", "Fokou", "Moukoko", "Ndi",
        "Nkongo", "Dongmo", "Talla", "Noumi"
    };

    private static readonly string[] Prenoms =
    {
        "Jean", "Marie", "Pierre", "Paul", "Jacques", "André", "Michel",
        "Françoise", "Catherine", "Émilie", "Sarah", "Grace", "Bénédicte",
        "Christian", "Samuel", "David", "Emmanuel", "Joseph"
    };

    // URLs Unsplash réalistes pour logements
    private static readonly string[] UnsplashImages =
    {
        "https://images.unsplash.com/photo-1502672260266-1c1efd283427?w=1200&q=80",
        "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=1200&q=80",
        "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=1200&q=80",
        "https://images.unsplash.com/photo-1484154218962-a196002b5911?w=1200&q=80",
        "https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=1200&q=80",
        "https://images.unsplash.com/photo-1486325212027-8081e485255e?w=1200&q=80",
        "https://images.unsplash.com/photo-1499793983690-e29da59ef1c2?w=1200&q=80",
        "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=1200&q=80",
        "https://images.unsplash.com/photo-1445019980598-944ed28699e5?w=1200&q=80",
        "https://images.unsplash.com/photo-1500511773475-3c44a5131e26?w=1200&q=80",
        "https://images.unsplash.com/photo-1493230534771-123b249b4e67?w=1200&q=80",
        "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=1200&q=80"
    };

    public DataSeeder(LocatCamDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Exécute le seed si la base de données est vide.
    /// </summary>
    public async Task RunAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Seed ignoré : des utilisateurs existent déjà.");
            return;
        }

        var faker = new Faker("fr");

        // 1. Créer les utilisateurs
        var users = new List<User>();

        // 3 admins
        for (int i = 0; i < 3; i++)
        {
            users.Add(CreateUser($"admin{i + 1}@locatcam.cm", "Admin@123", Role.Admin, $"Admin {i + 1}", "Système"));
        }

        // 6 bailleurs
        var bailleurs = new List<User>();
        for (int i = 0; i < 6; i++)
        {
            var nom = NomsBailleurs[i];
            var prenom = Prenoms[i % Prenoms.Length];
            var email = $"bailleur{i + 1}@locatcam.cm";
            var tel = GenererTelephoneCamerounais();
            var user = CreateUser(email, "Bailleur@123", Role.Bailleur, prenom, nom, tel);
            bailleurs.Add(user);
            users.Add(user);
        }

        // 6 étudiants
        var etudiants = new List<User>();
        for (int i = 0; i < 6; i++)
        {
            var nom = NomsEtudiants[i];
            var prenom = Prenoms[(i + 3) % Prenoms.Length];
            var email = $"etudiant{i + 1}@locatcam.cm";
            var tel = GenererTelephoneCamerounais();
            var user = CreateUser(email, "Etudiant@123", Role.Etudiant, prenom, nom, tel);
            etudiants.Add(user);
            users.Add(user);
        }

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Utilisateurs créés : {Count}", users.Count);

        // 2. Créer les annonces (40 au total, 20 Douala, 20 Yaoundé)
        var annonces = new List<Annonce>();
        var random = new Random();
        int annonceIndex = 0;

        foreach (var (ville, quartiers) in new[] { ("Douala", QuartiersDouala), ("Yaoundé", QuartiersYaounde) })
        {
            for (int i = 0; i < 20; i++)
            {
                var (quartier, lat, lng) = quartiers[random.Next(quartiers.Length)];
                var bailleur = bailleurs[random.Next(bailleurs.Count)];
                var typeIndex = random.Next(4);
                var type = (TypeAnnonce)typeIndex;

                decimal prix = type switch
                {
                    TypeAnnonce.Chambre => random.Next(15000, 40001),
                    TypeAnnonce.Studio => random.Next(35000, 80001),
                    TypeAnnonce.Appartement => random.Next(60000, 200001),
                    TypeAnnonce.Maison => random.Next(100000, 400001),
                    _ => 50000
                };

                var estMeuble = random.Next(100) < 30; // ~30% meublées
                var disponible = random.Next(100) < 70; // ~70% disponibles
                var estVerifie = random.Next(100) < 20; // ~20% vérifiées
                var nombrePieces = type switch
                {
                    TypeAnnonce.Chambre => 1,
                    TypeAnnonce.Studio => 1,
                    TypeAnnonce.Appartement => random.Next(2, 5),
                    TypeAnnonce.Maison => random.Next(4, 8),
                    _ => 2
                };

                var statut = disponible ? StatutAnnonce.Publiee : StatutAnnonce.Louee;

                var annonce = new Annonce
                {
                    Titre = GenererTitre(type, quartier, ville),
                    Description = GenererDescription(type, quartier, ville, estMeuble),
                    Prix = prix,
                    Type = type,
                    Quartier = quartier,
                    Ville = ville,
                    Adresse = $"{random.Next(1, 500)} Rue {faker.Address.StreetName()}, {quartier}, {ville}",
                    Latitude = lat + (random.NextDouble() - 0.5) * 0.01,
                    Longitude = lng + (random.NextDouble() - 0.5) * 0.01,
                    EstMeuble = estMeuble,
                    NombrePieces = nombrePieces,
                    Disponible = disponible,
                    UserId = bailleur.Id,
                    EstVerifie = estVerifie,
                    Statut = statut,
                    DateCreation = DateTime.UtcNow.AddDays(-random.Next(1, 90))
                };

                annonces.Add(annonce);
                annonceIndex++;
            }
        }

        await _context.Annonces.AddRangeAsync(annonces);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Annonces créées : {Count}", annonces.Count);

        // 3. Créer les photos (3 à 6 par annonce)
        var photos = new List<AnnoncePhoto>();
        foreach (var annonce in annonces)
        {
            int nbPhotos = random.Next(3, 7);
            var selectedImages = UnsplashImages.OrderBy(_ => random.Next()).Take(nbPhotos).ToList();
            for (int j = 0; j < selectedImages.Count; j++)
            {
                photos.Add(new AnnoncePhoto
                {
                    AnnonceId = annonce.Id,
                    UrlCloudinary = selectedImages[j],
                    PublicId = $"locatcam/annonces/{annonce.Id}_{j}",
                    Ordre = j,
                    EstPrincipale = j == 0,
                    DateCreation = DateTime.UtcNow
                });
            }
        }

        await _context.AnnoncePhotos.AddRangeAsync(photos);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Photos créées : {Count}", photos.Count);

        // 4. Créer les favoris (au moins 20, répartis entre les étudiants)
        var favoris = new List<Favori>();
        foreach (var etudiant in etudiants)
        {
            var annoncesEtudiant = annonces.Where(a => a.Ville == "Douala" || a.Ville == "Yaoundé").OrderBy(_ => random.Next()).Take(random.Next(3, 7)).ToList();
            foreach (var annonce in annoncesEtudiant)
            {
                favoris.Add(new Favori
                {
                    UserId = etudiant.Id,
                    AnnonceId = annonce.Id,
                    DateAjout = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                });
            }
        }

        // S'assurer d'au moins 20 favoris
        while (favoris.Count < 20)
        {
            var etudiant = etudiants[random.Next(etudiants.Count)];
            var annonce = annonces[random.Next(annonces.Count)];
            if (!favoris.Any(f => f.UserId == etudiant.Id && f.AnnonceId == annonce.Id))
            {
                favoris.Add(new Favori
                {
                    UserId = etudiant.Id,
                    AnnonceId = annonce.Id,
                    DateAjout = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                });
            }
        }

        await _context.Favoris.AddRangeAsync(favoris);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Favoris créés : {Count}", favoris.Count);

        // 5. Créer les messages (au moins 10, conversations étudiant <-> bailleur)
        var messages = new List<Message>();
        foreach (var etudiant in etudiants.Take(4))
        {
            var bailleur = bailleurs[random.Next(bailleurs.Count)];
            var annonce = annonces.First(a => a.UserId == bailleur.Id && a.Disponible);
            for (int i = 0; i < random.Next(2, 5); i++)
            {
                bool fromEtudiant = random.Next(2) == 0;
                messages.Add(new Message
                {
                    AnnonceId = annonce.Id,
                    ExpediteurId = fromEtudiant ? etudiant.Id : bailleur.Id,
                    DestinataireId = fromEtudiant ? bailleur.Id : etudiant.Id,
                    Contenu = GenererMessage(fromEtudiant),
                    DateEnvoi = DateTime.UtcNow.AddDays(-random.Next(1, 14)),
                    Lu = random.Next(100) < 60
                });
            }
        }

        // S'assurer d'au moins 10 messages
        while (messages.Count < 10)
        {
            var etudiant = etudiants[random.Next(etudiants.Count)];
            var bailleur = bailleurs[random.Next(bailleurs.Count)];
            var annonce = annonces.First(a => a.UserId == bailleur.Id);
            bool fromEtudiant = random.Next(2) == 0;
            messages.Add(new Message
            {
                AnnonceId = annonce.Id,
                ExpediteurId = fromEtudiant ? etudiant.Id : bailleur.Id,
                DestinataireId = fromEtudiant ? bailleur.Id : etudiant.Id,
                Contenu = GenererMessage(fromEtudiant),
                DateEnvoi = DateTime.UtcNow.AddDays(-random.Next(1, 14)),
                Lu = random.Next(100) < 60
            });
        }

        await _context.Messages.AddRangeAsync(messages);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Messages créés : {Count}", messages.Count);
        _logger.LogInformation("Seed terminé : {Users} users, {Annonces} annonces, {Favoris} favoris",
            users.Count, annonces.Count, favoris.Count);
    }

    private static User CreateUser(string email, string password, Role role, string prenom, string nom, string? telephone = null)
    {
        return new User
        {
            Nom = $"{prenom} {nom}",
            Email = email,
            Telephone = telephone ?? GenererTelephoneCamerounais(),
            MotDePasseHash = BCrypt.HashPassword(password),
            Role = role,
            EstVerifie = role == Role.Admin,
            DateCreation = DateTime.UtcNow.AddDays(-new Random().Next(1, 180))
        };
    }

    private static string GenererTelephoneCamerounais()
    {
        var random = new Random();
        var prefix = random.Next(6, 8); // 6 ou 7
        return $"+237 {prefix}{random.Next(10, 99)} {random.Next(100, 999)} {random.Next(100, 999)}";
    }

    private static string GenererTitre(TypeAnnonce type, string quartier, string ville)
    {
        var adjectifs = new[] { "Charmante", "Moderne", "Spacieuse", "Lumineuse", "Cosy", "Élégant", "Neuf", "Rénové" };
        var nomsType = new[] { "chambre", "studio", "appartement", "maison" };
        var adjectif = adjectifs[new Random().Next(adjectifs.Length)];
        return $"{adjectif} {nomsType[(int)type]} à {quartier}, {ville}";
    }

    private static string GenererDescription(TypeAnnonce type, string quartier, string ville, bool estMeuble)
    {
        var meublePhrase = estMeuble ? "Le logement est entièrement meublé et équipé." : "Le logement est proposé vide, à aménager selon vos besoins.";
        var disponibilite = "Disponible immédiatement.";
        return $"Très beau {type.ToString().ToLower()} situé dans le quartier prisé de {quartier} à {ville}. {meublePhrase} {disponibilite} Idéal pour étudiants et jeunes actifs. Proche des commerces et transports en commun.";
    }

    private static string GenererMessage(bool fromEtudiant)
    {
        if (fromEtudiant)
        {
            var messages = new[]
            {
                "Bonjour, l'annonce est-elle encore disponible ? Je suis intéressé.",
                "Bonjour, puis-je visiter le logement ce week-end ? Merci.",
                "Bonjour, le prix est-il négociable ? Je suis étudiant.",
                "Bonjour, je voudrais plus de photos du logement. Cordialement."
            };
            return messages[new Random().Next(messages.Length)];
        }
        else
        {
            var messages = new[]
            {
                "Bonjour, oui le logement est encore disponible. Quand souhaitez-vous visiter ?",
                "Bonjour, le prix est ferme mais nous pouvons discuter de la caution.",
                "Bonjour, je vous envoie les photos supplémentaires par WhatsApp.",
                "Bonjour, le logement est libre immédiatement. À bientôt."
            };
            return messages[new Random().Next(messages.Length)];
        }
    }
}
