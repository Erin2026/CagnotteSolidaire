using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class GetCagnotteQueryHandler(ICagnotteQueryRepository _repository) : IRequestHandler<GetCagnotteQuery, CagnotteDTO>
{
    public Task<CagnotteDTO> Handle(GetCagnotteQuery query, CancellationToken cancellationToken = default)
        => _repository.GetOne(query.CagnotteId);
}
