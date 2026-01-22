using Moq;
using FluentAssertions;
using CagnotteSolidaire.Application.Participations.Commands.CreateParticipation;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;

namespace CagnotteSolidaire.Application.Tests.Participations.Commands;

public class CreateParticipationCommandHandlerTests
{
    private readonly Mock<IParticipationRepository> _mockParticipationRepo;
    private readonly Mock<ICagnotteRepository> _mockCagnotteRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateParticipationCommandHandler _handler;

    public CreateParticipationCommandHandlerTests()
    {
        _mockParticipationRepo = new Mock<IParticipationRepository>();
        _mockCagnotteRepo = new Mock<ICagnotteRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _handler = new CreateParticipationCommandHandler(
            _mockParticipationRepo.Object,
            _mockCagnotteRepo.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateParticipation()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var participantId = "participant-123";
        var montant = 50m;

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            Description = "Test",
            ObjectifFinancier = 1000m,
            Statut = StatutCagnotte.Active,
            GestionnaireId = "gestionnaire-123"
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotteId,
            ParticipantId = participantId,
            Montant = montant,
            Commentaire = "Bon courage !"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        _mockParticipationRepo.Verify(x => x.AddAsync(It.Is<Participation>(p =>
            p.CagnotteId == cagnotteId &&
            p.ParticipantId == participantId &&
            p.Montant == montant &&
            p.Commentaire == "Bon courage !"
        )), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CagnotteNotFound_ShouldReturnFailure()
    {
        // Arrange
        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Cagnotte?)null);

        var command = new CreateParticipationCommand
        {
            CagnotteId = Guid.NewGuid(),
            ParticipantId = "participant-123",
            Montant = 50m
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cagnotte introuvable");
        
        _mockParticipationRepo.Verify(x => x.AddAsync(It.IsAny<Participation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CagnotteNotActive_ShouldReturnFailure()
    {
        // Arrange
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 1000m,
            Statut = StatutCagnotte.Cloturee, // Cagnotte fermée
            GestionnaireId = "gestionnaire-123"
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotte.Id,
            ParticipantId = "participant-123",
            Montant = 50m
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cette cagnotte n'est plus active");
    }

    [Fact]
    public async Task Handle_MontantInvalid_ShouldReturnFailure()
    {
        // Arrange
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 1000m,
            Statut = StatutCagnotte.Active,
            GestionnaireId = "gestionnaire-123"
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotte.Id,
            ParticipantId = "participant-123",
            Montant = 0m // Montant invalide
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Le montant doit être supérieur à 0");
    }
}
