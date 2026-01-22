using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Participations.Commands.CreateParticipation;

public record CreateParticipationCommand : ICommand<Result<Guid>>
{
    public required Guid CagnotteId { get; init; }
    public required string ParticipantId { get; init; }
    public required decimal Montant { get; init; }
    public string? Commentaire { get; init; }
}
