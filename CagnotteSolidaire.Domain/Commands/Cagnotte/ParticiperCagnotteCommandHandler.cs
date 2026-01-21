using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class ParticiperCagnotteCommandHandler(
    ICagnotteCommandRepository cagnotteRepository,
    IParticipationCommandRepository participationRepository)
    : IRequestHandler<ParticiperCagnotteCommand>
{
    public async Task Handle(
        ParticiperCagnotteCommand command,
        CancellationToken cancellationToken)
    {
        var cagnotte = await cagnotteRepository.GetById(
            command.CagnotteId,
            cancellationToken);

        if (cagnotte == null)
            throw new ApplicationException("Cagnotte not found");

        var participation = new Participation(
            0,
            command.ParticipantId,
            command.CagnotteId,
            command.Montant);

        cagnotte.AjouterParticipation(participation);

        await participationRepository.Create(
            participation,
            cancellationToken);
    }
}
