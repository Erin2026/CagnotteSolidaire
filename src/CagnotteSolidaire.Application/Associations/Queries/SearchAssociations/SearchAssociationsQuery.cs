using CagnotteSolidaire.Application.Common.Interfaces;

namespace CagnotteSolidaire.Application.Associations.Queries.SearchAssociations;

public record SearchAssociationsQuery : IQuery<List<AssociationSearchDto>>
{
    public required string SearchTerm { get; init; }
    public string Departement { get; init; } = "68";
}
