using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class GetCagnottesGestionnaireQueryHandler(ICagnotteQueryRepository repository)
    : IRequestHandler<GetCagnottesGestionnaireQuery, List<CagnottesDTO>>
{
    public Task<List<CagnottesDTO>> Handle(GetCagnottesGestionnaireQuery query, CancellationToken cancellationToken)
        => repository.GetByGestionnaire(query.GestionnaireId);
}
