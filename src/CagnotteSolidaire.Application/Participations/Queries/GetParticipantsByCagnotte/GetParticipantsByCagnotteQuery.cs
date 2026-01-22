using CagnotteSolidaire.Application.Common.Interfaces;

namespace CagnotteSolidaire.Application.Participations.Queries.GetParticipantsByCagnotte;

public record GetParticipantsByCagnotteQuery : IQuery<List<ParticipantDto>>
{
    public required Guid CagnotteId { get; init; }
}
