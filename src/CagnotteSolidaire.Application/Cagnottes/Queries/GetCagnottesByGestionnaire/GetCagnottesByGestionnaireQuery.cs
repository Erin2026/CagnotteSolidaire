using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;

namespace CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnottesByGestionnaire;

public record GetCagnottesByGestionnaireQuery : IQuery<List<CagnotteDto>>
{
    public required string GestionnaireId { get; init; }
}
