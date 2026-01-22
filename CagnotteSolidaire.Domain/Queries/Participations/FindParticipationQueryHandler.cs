using CagnotteSolidaire.Domain.Queries.Participations;
using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace LibRator.Domain.Queries.Participations;

public class FindParticipationQueryHandler(IParticipationQueryRepository _repository) : IRequestHandler<FindParticipationQuery, ParticipationsDTO[]>
{
    public Task<ParticipationsDTO[]> Handle(FindParticipationQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAll(query.Limit, query.Offset, query);
}
