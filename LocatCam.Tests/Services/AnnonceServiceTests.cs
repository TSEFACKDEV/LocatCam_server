namespace LocatCam.Tests.Services;

using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Enums;
using LocatCam.Core.Interfaces;
using LocatCam.Infrastructure.Services;
using Moq;

/// <summary>
/// Tests unitaires pour AnnonceService (recherche, filtres, bounding box).
/// </summary>
public class AnnonceServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAnnonceRepository> _mockAnnonceRepository;
    private readonly Mock<IAnnoncePhotoRepository> _mockAnnoncePhotoRepository;
    private readonly AnnonceService _service;

    public AnnonceServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAnnonceRepository = new Mock<IAnnonceRepository>();
        _mockAnnoncePhotoRepository = new Mock<IAnnoncePhotoRepository>();

        _mockUnitOfWork.Setup(u => u.Annonces).Returns(_mockAnnonceRepository.Object);
        _mockUnitOfWork.Setup(u => u.AnnoncePhotos).Returns(_mockAnnoncePhotoRepository.Object);

        _service = new AnnonceService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnFilteredAnnounces()
    {
        // Arrange
        var annonces = new List<Annonce>
        {
            new Annonce { Id = Guid.NewGuid(), Titre = "Chambre Bonapriso", Prix = 25000, Ville = "Douala", Quartier = "Bonapriso", Type = TypeAnnonce.Chambre, EstMeuble = true, Disponible = true, Statut = StatutAnnonce.Publiee },
            new Annonce { Id = Guid.NewGuid(), Titre = "Studio Bastos", Prix = 50000, Ville = "Yaoundé", Quartier = "Bastos", Type = TypeAnnonce.Studio, EstMeuble = false, Disponible = true, Statut = StatutAnnonce.Publiee }
        };

        _mockAnnonceRepository
            .Setup(r => r.GetPagedAsync(null, null, null, null, null, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((annonces, annonces.Count));

        // Act
        var (items, totalCount) = await _service.GetPagedAsync(null, null, null, null, null, null, null, 1, 20);

        // Assert
        Assert.Equal(2, totalCount);
        Assert.Equal(2, items.Count());
        Assert.Equal("Chambre Bonapriso", items.First().Titre);
    }

    [Fact]
    public async Task GetByBoundingBoxAsync_ShouldReturnAnnouncesInBox()
    {
        // Arrange
        var annonces = new List<Annonce>
        {
            new Annonce { Id = Guid.NewGuid(), Titre = "Appartement Douala", Latitude = 4.05, Longitude = 9.76, Statut = StatutAnnonce.Publiee, Disponible = true }
        };

        _mockAnnonceRepository
            .Setup(r => r.GetByBoundingBoxAsync(4.0, 4.1, 9.7, 9.8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(annonces);

        // Act
        var result = await _service.GetByBoundingBoxAsync(4.0, 4.1, 9.7, 9.8);

        // Assert
        Assert.Single(result);
        Assert.Equal(4.05, result.First().Latitude);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetBrouillonStatusAndUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new AnnonceCreateDto
        {
            Titre = "Test",
            Description = "Description",
            Prix = 30000,
            Type = "Chambre",
            Quartier = "Bonapriso",
            Ville = "Douala",
            NombrePieces = 1,
            Latitude = 4.05,
            Longitude = 9.76
        };

        Annonce capturedAnnonce = null!;
        _mockUnitOfWork.Setup(u => u.Annonces.AddAsync(It.IsAny<Annonce>(), It.IsAny<CancellationToken>()))
            .Callback<Annonce, CancellationToken>((a, _) => capturedAnnonce = a)
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockAnnonceRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => capturedAnnonce);

        // Act
        var result = await _service.CreateAsync(userId, dto);

        // Assert
        Assert.Equal(StatutAnnonce.Brouillon, capturedAnnonce.Statut);
        Assert.Equal(userId, capturedAnnonce.UserId);
        Assert.Equal(dto.Titre, capturedAnnonce.Titre);
        Assert.NotNull(result);
    }
}
