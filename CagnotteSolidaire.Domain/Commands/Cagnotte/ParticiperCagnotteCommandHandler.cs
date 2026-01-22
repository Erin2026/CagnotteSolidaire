using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class ParticiperCagnotteCommandHandler(
    ICagnotteCommandRepository _cagnotteRepository,
    IParticipationCommandRepository _participationRepository,
    IUtilisateurCommandRepository _participantRepository)
    : IRequestHandler<ParticiperCagnotteCommand, int>
{
    public async Task<int> Handle(
        ParticiperCagnotteCommand command,
        CancellationToken cancellationToken = default )
    {
        var participant = await _participantRepository.GetOne(command.ParticipantId)
            ?? throw new ApplicationException($"Cannot find participant with id '{command.ParticipantId}'");
        var cagnotte = await _cagnotteRepository.GetOne(command.CagnotteId)
            ?? throw new ApplicationException($"Cannot find cagnotte with id '{command.CagnotteId}'");

        Participation participation = new(0, participant, cagnotte, command.Montant);

        var participationId = await _participationRepository.Upsert(participation);
        
        return participationId;
    }
}
