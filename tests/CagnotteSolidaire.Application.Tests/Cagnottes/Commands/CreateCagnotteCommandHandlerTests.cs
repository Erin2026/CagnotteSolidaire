using Moq;
using FluentAssertions;
using CagnotteSolidaire.Application.Cagnottes.Commands.CreateCagnotte;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;

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
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCagnotte()
    {
        // Arrange
        var associationId = Guid.NewGuid();
        var gestionnaireId = "gestionnaire-123";
        
        var association = new Association
        {
            Id = associationId,
            Nom = "Association Test",
            SIREN = "123456789",
            Departement = "68"
        };

        _mockAssociationRepo
            .Setup(x => x.GetByIdAsync(associationId))
            .ReturnsAsync(association);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateCagnotteCommand
        {
            Nom = "Ma première cagnotte",
            Description = "Description de test",
            ObjectifFinancier = 1000m,
            AssociationId = associationId,
            GestionnaireId = gestionnaireId
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        _mockCagnotteRepo.Verify(x => x.AddAsync(It.Is<Cagnotte>(c =>
            c.Nom == command.Nom &&
            c.Description == command.Description &&
            c.ObjectifFinancier == command.ObjectifFinancier &&
            c.GestionnaireId == gestionnaireId &&
            c.Statut == StatutCagnotte.Active
        )), Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AssociationNotFound_ShouldReturnFailure()
    {
        // Arrange
        var associationId = Guid.NewGuid();
        
        _mockAssociationRepo
            .Setup(x => x.GetByIdAsync(associationId))
            .ReturnsAsync((Association?)null);

        var command = new CreateCagnotteCommand
        {
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 1000m,
            AssociationId = associationId,
            GestionnaireId = "gestionnaire-123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Association introuvable");
        
        _mockCagnotteRepo.Verify(x => x.AddAsync(It.IsAny<Cagnotte>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithImageUrl_ShouldCreateCagnotteWithImage()
    {
        // Arrange
        var associationId = Guid.NewGuid();
        var imageUrl = "https://example.com/image.jpg";
        
        var association = new Association { Id = associationId, Nom = "Test", SIREN = "123456789", Departement = "68" };
        
        _mockAssociationRepo.Setup(x => x.GetByIdAsync(associationId)).ReturnsAsync(association);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new CreateCagnotteCommand
        {
            Nom = "Test",
            Description = "Test",
            ObjectifFinancier = 1000m,
            ImageUrl = imageUrl,
            AssociationId = associationId,
            GestionnaireId = "gestionnaire-123"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCagnotteRepo.Verify(x => x.AddAsync(It.Is<Cagnotte>(c => c.ImageUrl == imageUrl)), Times.Once);
    }
}
