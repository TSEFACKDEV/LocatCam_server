namespace LocatCam.Infrastructure.Data;

using LocatCam.Core.Entities;
using LocatCam.Core.Enums;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Contexte de base de données Entity Framework pour LocatCam.
/// </summary>
public class LocatCamDbContext : DbContext
{
    /// <summary>Constructeur recevant les options du contexte.</summary>
    public LocatCamDbContext(DbContextOptions<LocatCamDbContext> options) : base(options)
    {
    }

    /// <summary>Utilisateurs de l'application.</summary>
    public DbSet<User> Users { get; set; }

    /// <summary>Annonces de logement.</summary>
    public DbSet<Annonce> Annonces { get; set; }

    /// <summary>Photos des annonces (Cloudinary).</summary>
    public DbSet<AnnoncePhoto> AnnoncePhotos { get; set; }

    /// <summary>Favoris des utilisateurs.</summary>
    public DbSet<Favori> Favoris { get; set; }

    /// <summary>Messages entre utilisateurs.</summary>
    public DbSet<Message> Messages { get; set; }

    /// <summary>Signalements d'annonces.</summary>
    public DbSet<Signalement> Signalements { get; set; }

    /// <summary>Configuration du modèle et des contraintes.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- User ---
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Telephone).IsUnique();
            entity.Property(u => u.Nom).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.Telephone).IsRequired().HasMaxLength(20);
            entity.Property(u => u.MotDePasseHash).IsRequired();
            entity.Property(u => u.Role).IsRequired();
            entity.Property(u => u.PhotoUrl).HasMaxLength(500);
        });

        // --- Annonce ---
        modelBuilder.Entity<Annonce>(entity =>
        {
            entity.HasIndex(a => a.UserId);
            entity.HasIndex(a => a.Ville);
            entity.HasIndex(a => a.Quartier);
            entity.HasIndex(a => a.Prix);
            entity.HasIndex(a => a.Statut);
            entity.HasIndex(a => new { a.Latitude, a.Longitude });

            entity.Property(a => a.Titre).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Description).IsRequired().HasMaxLength(2000);
            entity.Property(a => a.Prix).HasColumnType("decimal(18,2)");
            entity.Property(a => a.Type).IsRequired();
            entity.Property(a => a.Quartier).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Ville).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Adresse).HasMaxLength(500);
            entity.Property(a => a.EstMeuble).IsRequired();
            entity.Property(a => a.NombrePieces).IsRequired();
            entity.Property(a => a.Disponible).IsRequired();
            entity.Property(a => a.EstVerifie).IsRequired();

            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- AnnoncePhoto ---
        modelBuilder.Entity<AnnoncePhoto>(entity =>
        {
            entity.HasIndex(p => p.AnnonceId);
            entity.Property(p => p.UrlCloudinary).IsRequired().HasMaxLength(1000);
            entity.Property(p => p.PublicId).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Ordre).IsRequired();
            entity.Property(p => p.EstPrincipale).IsRequired();

            entity.HasOne(p => p.Annonce)
                .WithMany(a => a.Photos)
                .HasForeignKey(p => p.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Favori ---
        modelBuilder.Entity<Favori>(entity =>
        {
            entity.HasIndex(f => new { f.UserId, f.AnnonceId }).IsUnique();
            entity.HasIndex(f => f.UserId);
            entity.HasIndex(f => f.AnnonceId);
            entity.Property(f => f.DateAjout).IsRequired();

            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Annonce)
                .WithMany(a => a.Favoris)
                .HasForeignKey(f => f.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Message ---
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(m => m.AnnonceId);
            entity.HasIndex(m => m.ExpediteurId);
            entity.HasIndex(m => m.DestinataireId);
            entity.HasIndex(m => m.DateEnvoi);
            entity.Property(m => m.Contenu).IsRequired().HasMaxLength(2000);
            entity.Property(m => m.DateEnvoi).IsRequired();
            entity.Property(m => m.Lu).IsRequired();

            entity.HasOne(m => m.Annonce)
                .WithMany(a => a.Messages)
                .HasForeignKey(m => m.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Expediteur)
                .WithMany()
                .HasForeignKey(m => m.ExpediteurId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Destinataire)
                .WithMany()
                .HasForeignKey(m => m.DestinataireId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Signalement ---
        modelBuilder.Entity<Signalement>(entity =>
        {
            entity.HasIndex(s => s.AnnonceId);
            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => s.DateSignalement);
            entity.Property(s => s.Raison).IsRequired().HasMaxLength(500);
            entity.Property(s => s.DateSignalement).IsRequired();
            entity.Property(s => s.Traite).IsRequired();

            entity.HasOne(s => s.Annonce)
                .WithMany(a => a.Signalements)
                .HasForeignKey(s => s.AnnonceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
