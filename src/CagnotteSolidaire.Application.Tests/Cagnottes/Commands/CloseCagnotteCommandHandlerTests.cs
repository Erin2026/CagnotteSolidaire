using CagnotteSolidaire.Application.Cagnottes.Commands.CloseCagnotte;
using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

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
            _mockEmailService.Object
        );
    }

    [Fact]
    public async Task Handle_ObjectifAtteint_ShouldCloseCagnotte()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var gestionnaireId = Guid.NewGuid().ToString();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            ObjectifFinancier = 1000,
            GestionnaireId = gestionnaireId,
            Statut = StatutCagnotte.Active,
            Participations = new List<Participation>
            {
                new() { Montant = 600 },
                new() { Montant = 500 } // Total = 1100, objectif atteint !
            }
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        _mockCagnotteRepo
            .Setup(r => r.UpdateAsync(It.IsAny<Cagnotte>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotteId,
            GestionnaireId = gestionnaireId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        cagnotte.Statut.Should().Be(StatutCagnotte.Cloturee);
        cagnotte.DateCloture.Should().NotBeNull();

        _mockCagnotteRepo.Verify(
            r => r.UpdateAsync(It.IsAny<Cagnotte>()),
            Times.Once
        );

        _mockEmailService.Verify(
            e => e.SendCagnotteClosedEmailAsync(It.IsAny<Cagnotte>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ObjectifNonAtteint_ShouldReturnFailure()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var gestionnaireId = Guid.NewGuid().ToString();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            ObjectifFinancier = 1000,
            GestionnaireId = gestionnaireId,
            Statut = StatutCagnotte.Active,
            Participations = new List<Participation>
            {
                new() { Montant = 300 },
                new() { Montant = 200 } // Total = 500, objectif non atteint
            }
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotteId,
            GestionnaireId = gestionnaireId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("objectif");

        _mockCagnotteRepo.Verify(
            r => r.UpdateAsync(It.IsAny<Cagnotte>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_NotOwner_ShouldReturnFailure()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var realGestionnaireId = Guid.NewGuid().ToString();
        var fakeGestionnaireId = Guid.NewGuid().ToString();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            GestionnaireId = realGestionnaireId,
            Statut = StatutCagnotte.Active
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotteId,
            GestionnaireId = fakeGestionnaireId // Pas le bon gestionnaire !
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("autorise");

        _mockCagnotteRepo.Verify(
            r => r.UpdateAsync(It.IsAny<Cagnotte>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_CagnotteNotActive_ShouldReturnFailure()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var gestionnaireId = Guid.NewGuid().ToString();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            GestionnaireId = gestionnaireId,
            Statut = StatutCagnotte.Cloturee // Deja cloturee
        };

        _mockCagnotteRepo
            .Setup(r => r.GetWithParticipationsAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var command = new CloseCagnotteCommand
        {
            CagnotteId = cagnotteId,
            GestionnaireId = gestionnaireId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("active");
    }
}
