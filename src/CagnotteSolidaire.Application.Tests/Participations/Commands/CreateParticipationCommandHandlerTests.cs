using CagnotteSolidaire.Application.Participations.Commands.CreateParticipation;
using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

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
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateParticipation()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();
        var participantId = Guid.NewGuid().ToString();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Nom = "Test Cagnotte",
            Statut = StatutCagnotte.Active
        };

        _mockCagnotteRepo
            .Setup(r => r.GetByIdAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        _mockParticipationRepo
            .Setup(r => r.AddAsync(It.IsAny<Participation>()))
            .ReturnsAsync((Participation p) => p);


        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotteId,
            ParticipantId = participantId,
            Montant = 50,
            Commentaire = "Bon courage !"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _mockParticipationRepo.Verify(
            r => r.AddAsync(It.Is<Participation>(p =>
                p.CagnotteId == cagnotteId &&
                p.ParticipantId == participantId &&
                p.Montant == 50 &&
                p.Commentaire == "Bon courage !"
            )),
            Times.Once
        );

        _mockUnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_CagnotteNotFound_ShouldReturnFailure()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();

        _mockCagnotteRepo
            .Setup(r => r.GetByIdAsync(cagnotteId))
            .ReturnsAsync((Cagnotte?)null);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotteId,
            ParticipantId = Guid.NewGuid().ToString(),
            Montant = 50
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cagnotte introuvable");

        _mockParticipationRepo.Verify(
            r => r.AddAsync(It.IsAny<Participation>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_CagnotteNotActive_ShouldReturnFailure()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Statut = StatutCagnotte.Cloturee // Cagnotte fermee
        };

        _mockCagnotteRepo
            .Setup(r => r.GetByIdAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotteId,
            ParticipantId = Guid.NewGuid().ToString(),
            Montant = 50
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("active");

        _mockParticipationRepo.Verify(
            r => r.AddAsync(It.IsAny<Participation>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_InvalidMontant_ShouldReturnFailure()
    {
        // Arrange
        var cagnotteId = Guid.NewGuid();

        var cagnotte = new Cagnotte
        {
            Id = cagnotteId,
            Statut = StatutCagnotte.Active
        };

        _mockCagnotteRepo
            .Setup(r => r.GetByIdAsync(cagnotteId))
            .ReturnsAsync(cagnotte);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotteId,
            ParticipantId = Guid.NewGuid().ToString(),
            Montant = -10 // Montant invalide
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("montant");

        _mockParticipationRepo.Verify(
            r => r.AddAsync(It.IsAny<Participation>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handle_MontantZeroOrNegative_ShouldReturnFailure(decimal montant)
    {
        // Arrange
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Statut = StatutCagnotte.Active
        };

        _mockCagnotteRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cagnotte);

        var command = new CreateParticipationCommand
        {
            CagnotteId = cagnotte.Id,
            ParticipantId = Guid.NewGuid().ToString(),
            Montant = montant
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("montant");
    }
}
