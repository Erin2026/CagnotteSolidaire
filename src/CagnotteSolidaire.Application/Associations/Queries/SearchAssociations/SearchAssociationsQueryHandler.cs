using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Associations.Queries.SearchAssociations;

public class SearchAssociationsQueryHandler : IRequestHandler<SearchAssociationsQuery, List<AssociationSearchDto>>
{
    private readonly IJOAssociationsService _joService;

    public SearchAssociationsQueryHandler(IJOAssociationsService joService)
    {
        _joService = joService;
    }

    public async Task<List<AssociationSearchDto>> Handle(SearchAssociationsQuery request, CancellationToken cancellationToken)
    {
        var result = await _joService.SearchAssociationsAsync(request.SearchTerm, request.Departement);

        return result.Records.Select(r => new AssociationSearchDto
        {
            Nom = r.Fields.Nom ?? string.Empty,
            SIREN = r.Fields.SIREN,
            RNA = r.Fields.RNA,
            Ville = r.Fields.Ville,
            CodePostal = r.Fields.CodePostal,
            Adresse = $"{r.Fields.NumeroVoie} {r.Fields.LibelleVoie}".Trim()
        }).ToList();
    }
}


