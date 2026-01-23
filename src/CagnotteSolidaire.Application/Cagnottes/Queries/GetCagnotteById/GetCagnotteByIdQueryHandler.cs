using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;

public class GetCagnotteByIdQueryHandler : IRequestHandler<GetCagnotteByIdQuery, CagnotteDto?>
{
    private readonly ICagnotteRepository _cagnotteRepository;

    public GetCagnotteByIdQueryHandler(ICagnotteRepository cagnotteRepository)
    {
        _cagnotteRepository = cagnotteRepository;
    }

    public async Task<CagnotteDto?> Handle(GetCagnotteByIdQuery request, CancellationToken cancellationToken)
    {
        // Utiliser GetWithParticipationsAsync pour calculer les montants et nombres
        var cagnotte = await _cagnotteRepository.GetWithParticipationsAsync(request.Id);
        
        if (cagnotte == null)
            return null;

        return new CagnotteDto
        {
            Id = cagnotte.Id,
            Nom = cagnotte.Nom,
            Description = cagnotte.Description,
            ObjectifFinancier = cagnotte.ObjectifFinancier,
            MontantActuel = cagnotte.MontantCollecte,  // Calcule depuis les participations
            ImageUrl = cagnotte.ImageUrl,
            Statut = cagnotte.Statut.ToString(),
            DateCreation = cagnotte.DateCreation,
            NomAssociation = cagnotte.Gestionnaire?.Association?.Nom ?? cagnotte.Gestionnaire?.Email ?? string.Empty,
            NombreParticipants = cagnotte.NombreParticipants  // Compte les participations
        };
    }

}

