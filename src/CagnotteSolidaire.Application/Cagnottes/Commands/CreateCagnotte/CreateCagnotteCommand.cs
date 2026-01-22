using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Cagnottes.Commands.CreateCagnotte;

public record CreateCagnotteCommand : ICommand<Result<Guid>>
{
    public required string Nom { get; init; }
    public required string Description { get; init; }
    public required decimal ObjectifFinancier { get; init; }
    public string? ImageUrl { get; init; }
    public required Guid AssociationId { get; init; }
    public required string GestionnaireId { get; init; }
}

