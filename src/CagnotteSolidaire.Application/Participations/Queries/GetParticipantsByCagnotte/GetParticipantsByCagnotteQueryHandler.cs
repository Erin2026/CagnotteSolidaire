using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Participations.Queries.GetParticipantsByCagnotte;

public class GetParticipantsByCagnotteQueryHandler : IRequestHandler<GetParticipantsByCagnotteQuery, List<ParticipantDto>>
{
    private readonly IParticipationRepository _participationRepository;

    public GetParticipantsByCagnotteQueryHandler(IParticipationRepository participationRepository)
    {
        _participationRepository = participationRepository;
    }

    public async Task<List<ParticipantDto>> Handle(GetParticipantsByCagnotteQuery request, CancellationToken cancellationToken)
    {
        var participations = await _participationRepository.GetByCagnotteIdAsync(request.CagnotteId);

        return participations.Select(p => new ParticipantDto
        {
            Id = p.Id,
            NomComplet = p.Participant?.NomComplet ?? "Inconnu",
            Email = p.Participant?.Email ?? string.Empty,
            Montant = p.Montant,
            Commentaire = p.Commentaire,
            DateParticipation = p.DateParticipation
        }).ToList();
    }
}
