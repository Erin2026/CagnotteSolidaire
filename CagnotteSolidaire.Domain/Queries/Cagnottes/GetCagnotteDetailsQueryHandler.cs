using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class GetCagnotteDetailsQueryHandler(ICagnotteQueryRepository repository)
    : IRequestHandler<GetCagnotteDetailsQuery, CagnotteDTO>
{
    public Task<CagnotteDTO> Handle(GetCagnotteDetailsQuery query, CancellationToken cancellationToken)
        => repository.GetOne(query.CagnotteId, cancellationToken);
}
