using CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

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
    public async Task Handle_CagnotteExists_ShouldReturnCagnotteDto()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var gestionnaireId = Guid.NewGuid().ToString();

        var gestionnaire = new ApplicationUser
        {
            Id = gestionnaireId,
            Email = "gestionnaire@test.com",
            Nom = "Dupont",
            Prenom = "Jean",
            Association = new Association { Nom = "Mon Association" }
        };

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            Description = "Description de test",
            ObjectifFinancier = 1000,
            ImageUrl = "http://test.com/image.jpg",
            Statut = StatutCagnotte.Active,
            DateCreation = DateTime.UtcNow,
            GestionnaireId = gestionnaireId,
            Gestionnaire = gestionnaire,
            Participations = new List<Participation>
            {
                new() { Montant = 300 },
                new() { Montant = 200 },
                new() { Montant = 100 }
            }
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var query = new GetCagnotteByIdQuery { Id = cagnotteId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(cagnotteId);
        result.Nom.Should().Be("Test Cagnotte");
        result.Description.Should().Be("Description de test");
        result.ObjectifFinancier.Should().Be(1000);
        result.MontantActuel.Should().Be(600); // 300 + 200 + 100
        result.NombreParticipants.Should().Be(3);
        result.Statut.Should().Be("Active");
        result.NomAssociation.Should().Be("Mon Association");
    }

    [Fact]
    public async Task Handle_CagnotteNotFound_ShouldReturnNull()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync((Cagnotte?)null);

        var query = new GetCagnotteByIdQuery { Id = cagnotteId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CagnotteWithoutParticipations_ShouldReturnDtoWithZeroMontant()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Cagnotte vide",
            ObjectifFinancier = 1000,
            Statut = StatutCagnotte.Active,
            Participations = new List<Participation>() // Vide
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var query = new GetCagnotteByIdQuery { Id = cagnotteId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.MontantActuel.Should().Be(0);
        result.NombreParticipants.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CagnotteCloturee_ShouldReturnClosedStatus()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Cagnotte cloturee",
            ObjectifFinancier = 1000,
            Statut = StatutCagnotte.Cloturee,
            DateCloture = DateTime.UtcNow,
            Participations = new List<Participation>
            {
                new() { Montant = 1100 } // Objectif depasse
            }
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var query = new GetCagnotteByIdQuery { Id = cagnotteId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Statut.Should().Be("Cloturee");
        result.MontantActuel.Should().Be(1100);
    }
}
