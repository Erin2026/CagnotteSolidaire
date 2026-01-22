using Moq;
using FluentAssertions;
using CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;

namespace CagnotteSolidaire.Application.Tests.Cagnottes.Queries;

public class GetCagnotteByIdQueryHandlerTests
{
    private readonly Mock<ICagnotteRepository> _mockCagnotteRepo;
    private readonly GetCagnotteByIdQueryHandler _handler;

    public GetCagnotteByIdQueryHandlerTests()
    {
        _mockCagnotteRepo = new Mock<ICagnotteRepository>();
        _handler = new GetCagnotteByIdQueryHandler(_mockCagnotteRepo.Object);
    }

    [Fact]
    public async Task Handle_ExistingCagnotte_ShouldReturnDto()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            Description = "Une description de test",
            ObjectifFinancier = 1000m,
            Statut = StatutCagnotte.Active,
            DateCreation = DateTime.UtcNow,
            GestionnaireId = "gestionnaire-123",
            Gestionnaire = new ApplicationUser
            {
                Id = "gestionnaire-123",
                Email = "gestionnaire@test.com",
                Nom = "Dupont",
                Prenom = "Jean"
            },
            Participations = new List<Participation>
            {
                new() { Id = Guid.NewGuid(), Montant = 100m, ParticipantId = "p1" },
                new() { Id = Guid.NewGuid(), Montant = 200m, ParticipantId = "p2" }
            }
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var query = new GetCagnotteByIdQuery { Id = cagnotteId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(cagnotteId);
        result.Nom.Should().Be("Test Cagnotte");
        result.Description.Should().Be("Une description de test");
        result.ObjectifFinancier.Should().Be(1000m);
        result.MontantActuel.Should().Be(300m); // 100 + 200
        result.Statut.Should().Be(StatutCagnotte.Active.ToString());
        result.NombreParticipants.Should().Be(2);
    }

    [Fact]
    public async Task Handle_NonExistingCagnotte_ShouldReturnNull()
    {
        // Arrange
        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Cagnotte?)null);

        var query = new GetCagnotteByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CagnotteWithoutParticipations_ShouldReturnDtoWithZeroAmount()
    {
        // Arrange
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 1000m,
            Statut = StatutCagnotte.Active,
            GestionnaireId = "gestionnaire-123",
            Gestionnaire = new ApplicationUser { Email = "test@test.com", Nom = "Test", Prenom = "Test" },
            Participations = new List<Participation>()
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var query = new GetCagnotteByIdQuery { Id = cagnotte.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.MontantActuel.Should().Be(0m);
        result.NombreParticipants.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CagnotteWithImage_ShouldIncludeImageUrl()
    {
        // Arrange
        var imageUrl = "https://example.com/image.jpg";
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 1000m,
            ImageUrl = imageUrl,
            Statut = StatutCagnotte.Active,
            GestionnaireId = "gestionnaire-123",
            Gestionnaire = new ApplicationUser { Email = "test@test.com", Nom = "Test", Prenom = "Test" }
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var query = new GetCagnotteByIdQuery { Id = cagnotte.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ImageUrl.Should().Be(imageUrl);
    }
}
