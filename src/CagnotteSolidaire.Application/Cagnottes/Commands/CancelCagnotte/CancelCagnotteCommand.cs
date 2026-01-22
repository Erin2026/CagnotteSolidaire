using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Cagnottes.Commands.CancelCagnotte;

public record CancelCagnotteCommand : ICommand<Result>
{
    public required Guid CagnotteId { get; init; }
    public required string GestionnaireId { get; init; }
}
