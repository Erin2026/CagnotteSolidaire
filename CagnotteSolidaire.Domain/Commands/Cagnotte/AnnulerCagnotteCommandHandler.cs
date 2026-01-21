using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class AnnulerCagnotteCommandHandler(
    ICagnotteCommandRepository repository)
    : IRequestHandler<AnnulerCagnotteCommand>
{
    public async Task Handle(
        AnnulerCagnotteCommand command,
        CancellationToken cancellationToken)
    {
        var cagnotte = await repository.GetById(
            command.CagnotteId,
            cancellationToken);

        if (cagnotte == null)
            throw new ApplicationException("Cagnotte not found");

        cagnotte.Annuler();

        await repository.Upsert(cagnotte, cancellationToken);
    }
}
