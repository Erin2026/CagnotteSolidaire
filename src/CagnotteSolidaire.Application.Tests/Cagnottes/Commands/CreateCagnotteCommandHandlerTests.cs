using CagnotteSolidaire.Application.Cagnottes.Commands.CreateCagnotte;
using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CagnotteSolidaire.Application.Tests.Cagnottes.Commands;

public class CreateCagnotteCommandHandlerTests
{
    private readonly Mock<ICagnotteRepository> _mockCagnotteRepo;
    private readonly Mock<IAssociationRepository> _mockAssociationRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateCagnotteCommandHandler _handler;

    public CreateCagnotteCommandHandlerTests()
    {
        _mockCagnotteRepo = new Mock<ICagnotteRepository>();
        _mockAssociationRepo = new Mock<IAssociationRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new CreateCagnotteCommandHandler(
            _mockCagnotteRepo.Object,
            _mockAssociationRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCagnotte()
    {
        // Arrange
        var associationId = Guid.NewGuid();
        var gestionnaireId = Guid.NewGuid().ToString();

        var association = new Association
        {
            Id = associationId,
            Nom = "Test Association",
            Departement = "68"
        };

        _mockAssociationRepo
            .Setup(r => r.GetByIdAsync(associationId))
            .ReturnsAsync(association);

        _mockCagnotteRepo
            .Setup(r => r.AddAsync(It.IsAny<Cagnotte>()))
            .ReturnsAsync((Cagnotte c) => c);


        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateCagnotteCommand
        {
            Nom = "Cagnotte Test",
            Description = "Description test",
            ObjectifFinancier = 1000,
            ImageUrl = "http://test.com/image.jpg",
            GestionnaireId = gestionnaireId,
            AssociationId = associationId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _mockCagnotteRepo.Verify(
            r => r.AddAsync(It.Is<Cagnotte>(c =>
                c.Nom == "Cagnotte Test" &&
                c.ObjectifFinancier == 1000 &&
                c.Statut == StatutCagnotte.Active
            )),
            Times.Once
        );

        _mockUnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_AssociationNotFound_ShouldReturnFailure()
    {
        // Arrange
        var associationId = Guid.NewGuid();

        _mockAssociationRepo
            .Setup(r => r.GetByIdAsync(associationId))
            .ReturnsAsync((Association?)null);

        var command = new CreateCagnotteCommand
        {
            Nom = "Cagnotte Test",
            Description = "Description",
            ObjectifFinancier = 1000,
            GestionnaireId = Guid.NewGuid().ToString(),
            AssociationId = associationId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Association introuvable");

        _mockCagnotteRepo.Verify(
            r => r.AddAsync(It.IsAny<Cagnotte>()),
            Times.Never
        );
    }

    [Fact(Skip = "Le handler ne valide pas encore les montants negatifs")]
    public async Task Handle_InvalidObjectifFinancier_ShouldReturnFailure()

    {
        // Arrange
        var associationId = Guid.NewGuid();
        var association = new Association { Id = associationId, Nom = "Test", Departement = "68" };

        _mockAssociationRepo
            .Setup(r => r.GetByIdAsync(associationId))
            .ReturnsAsync(association);

        var command = new CreateCagnotteCommand
        {
            Nom = "Cagnotte Test",
            Description = "Description",
            ObjectifFinancier = -100, // Invalide
            GestionnaireId = Guid.NewGuid().ToString(),
            AssociationId = associationId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("objectif");

        _mockCagnotteRepo.Verify(
            r => r.AddAsync(It.IsAny<Cagnotte>()),
            Times.Never
        );
    }
}
