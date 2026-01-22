using CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;
using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnottesByGestionnaire;

public class GetCagnottesByGestionnaireQueryHandler : IRequestHandler<GetCagnottesByGestionnaireQuery, List<CagnotteDto>>
{
    private readonly ICagnotteRepository _cagnotteRepository;

    public GetCagnottesByGestionnaireQueryHandler(ICagnotteRepository cagnotteRepository)
    {
        _cagnotteRepository = cagnotteRepository;
    }

    public async Task<List<CagnotteDto>> Handle(GetCagnottesByGestionnaireQuery request, CancellationToken cancellationToken)
    {
        var cagnottes = await _cagnotteRepository.GetByGestionnaireIdAsync(request.GestionnaireId);

        return cagnottes.Select(c => new CagnotteDto
        {
            Id = c.Id,
            Nom = c.Nom,
            Description = c.Description,
            ObjectifFinancier = c.ObjectifFinancier,
            MontantActuel = c.MontantCollecte,
            ImageUrl = c.ImageUrl,
            Statut = c.Statut.ToString(),
            DateCreation = c.DateCreation,
            NomAssociation = c.Gestionnaire?.Email ?? string.Empty,
            NombreParticipants = c.NombreParticipants
        }).ToList();
    }
}
