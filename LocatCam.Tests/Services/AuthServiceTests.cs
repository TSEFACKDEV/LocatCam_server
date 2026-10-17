namespace LocatCam.Tests.Services;

using LocatCam.Core.Dtos;
using LocatCam.Core.Entities;
using LocatCam.Core.Enums;
using LocatCam.Core.Interfaces;
using LocatCam.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;

/// <summary>
/// Tests unitaires pour AuthService (inscription, connexion).
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly IConfiguration _configuration;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTokenService = new Mock<ITokenService>();
        _mockUserRepository = new Mock<IUserRepository>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);

        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:AccessTokenExpirationMinutes", "60"}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _service = new AuthService(_mockUnitOfWork.Object, _mockTokenService.Object, _configuration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowIfEmailExists()
    {
        // Arrange
        var dto = new RegisterRequestDto { Email = "test@locatcam.cm", MotDePasse = "Password123", Role = "Etudiant" };
        _mockUserRepository.Setup(r => r.EmailExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowIfWrongPassword()
    {
        // Arrange
        var dto = new LoginRequestDto { Email = "test@locatcam.cm", MotDePasse = "WrongPassword" };
        var user = new User { Id = Guid.NewGuid(), Email = dto.Email, MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword") };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokenIfPasswordCorrect()
    {
        // Arrange
        var dto = new LoginRequestDto { Email = "test@locatcam.cm", MotDePasse = "CorrectPassword" };
        var user = new User { Id = Guid.NewGuid(), Email = dto.Email, Nom = "Test User", Role = Role.Etudiant, MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse) };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockTokenService.Setup(t => t.GenerateAccessToken(user)).Returns("fake-access-token");
        _mockTokenService.Setup(t => t.GenerateRefreshToken()).Returns("fake-refresh-token");

        // Act
        var result = await _service.LoginAsync(dto);

        // Assert
        Assert.Equal("fake-access-token", result.AccessToken);
        Assert.Equal("fake-refresh-token", result.RefreshToken);
        Assert.Equal(user.Id, result.User.Id);
    }
}
