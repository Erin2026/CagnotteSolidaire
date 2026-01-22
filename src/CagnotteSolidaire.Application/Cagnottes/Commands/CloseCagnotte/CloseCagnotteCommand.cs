using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Cagnottes.Commands.CloseCagnotte;

public record CloseCagnotteCommand : ICommand<Result>
{
    public required Guid CagnotteId { get; init; }
    public required string GestionnaireId { get; init; }
}
