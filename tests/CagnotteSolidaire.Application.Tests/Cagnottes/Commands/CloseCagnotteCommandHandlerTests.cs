using Moq;
using FluentAssertions;
using CagnotteSolidaire.Application.Cagnottes.Commands.CloseCagnotte;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;

namespace CagnotteSolidaire.Application.Tests.Cagnottes.Commands;

public class CloseCagnotteCommandHandlerTests
{
    private readonly Mock<ICagnotteRepository> _mockCagnotteRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly CloseCagnotteCommandHandler _handler;

    public CloseCagnotteCommandHandlerTests()
    {
        _mockCagnotteRepo = new Mock<ICagnotteRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEmailService = new Mock<IEmailService>();
        
        _handler = new CloseCagnotteCommandHandler(
            _mockCagnotteRepo.Object,
            _mockUnitOfWork.Object,
            _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCloseCagnotteAndSendEmails()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var gestionnaireId = "gestionnaire-123";

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            Description = "Test",
            ObjectifFinancier = 100m,
            Statut = StatutCagnotte.Active,
            GestionnaireId = gestionnaireId,
            Participations = new List<Participation>
            {
                new() { Id = Guid.NewGuid(), Montant = 60m, ParticipantId = "p1" },
                new() { Id = Guid.NewGuid(), Montant = 50m, ParticipantId = "p2" }
            }
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotteId,
            GestionnaireId = gestionnaireId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        cagnotte.Statut.Should().Be(StatutCagnotte.Cloturee);
        cagnotte.DateCloture.Should().NotBeNull();

        _mockCagnotteRepo.Verify(x => x.UpdateAsync(cagnotte), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockEmailService.Verify(x => x.SendCagnotteClosedEmailAsync(cagnotte), Times.Once);
    }

    [Fact]
    public async Task Handle_CagnotteNotFound_ShouldReturnFailure()
    {
        // Arrange
        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Cagnotte?)null);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = Guid.NewGuid(),
            GestionnaireId = "gestionnaire-123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cagnotte introuvable");
        
        _mockEmailService.Verify(x => x.SendCagnotteClosedEmailAsync(It.IsAny<Cagnotte>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NotAuthorized_ShouldReturnFailure()
    {
        // Arrange
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 100m,
            Statut = StatutCagnotte.Active,
            GestionnaireId = "gestionnaire-123"
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotte.Id,
            GestionnaireId = "autre-gestionnaire-456" // Différent gestionnaire
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Vous n'êtes pas autorisé à clôturer cette cagnotte");
    }

    [Fact]
    public async Task Handle_ObjectiveNotReached_ShouldReturnFailure()
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
            Participations = new List<Participation>
            {
                new() { Id = Guid.NewGuid(), Montant = 100m, ParticipantId = "p1" }
            }
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotte.Id,
            GestionnaireId = "gestionnaire-123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("L'objectif n'est pas atteint");
    }

    [Fact]
    public async Task Handle_AlreadyClosed_ShouldReturnFailure()
    {
        // Arrange
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 100m,
            Statut = StatutCagnotte.Cloturee, // Déjà clôturée
            GestionnaireId = "gestionnaire-123"
        };

        _mockCagnotteRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotte.Id,
            GestionnaireId = "gestionnaire-123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cette cagnotte n'est plus active");
    }
}
