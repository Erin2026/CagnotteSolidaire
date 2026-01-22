using CagnotteSolidaire.Domain.Queries.Participations;
using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace LibRator.Domain.Queries.Participations;

public class FindParticipationQuery(
    int? cagnotteId = null,
    int? participantId = null,
    decimal? minMontant = null,
    decimal? maxMontant = null
) : IRequest<ParticipationsDTO[]>
{
    public int Limit { get; set; } = 10;
    public int Offset { get; set; } = 0;
    public int? CagnotteId { get; } = cagnotteId;
    public int? ParticipantId { get; } = participantId;
    public decimal? MinMontant { get; } = minMontant;
    public decimal? MaxMontant { get; } = maxMontant;
}

