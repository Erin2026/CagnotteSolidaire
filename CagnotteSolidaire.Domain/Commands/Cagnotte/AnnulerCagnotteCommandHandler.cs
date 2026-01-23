using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class AnnulerCagnotteCommandHandler(
    ICagnotteCommandRepository repository)
    : IRequestHandler<AnnulerCagnotteCommand>
{
    public async Task Handle(
        AnnulerCagnotteCommand command,
        CancellationToken cancellationToken= default)
    {
        var cagnotte = await repository.GetOne(
            command.CagnotteId)
        ?? throw new ApplicationException($"Cannot find cagnotte with id '{command.CagnotteId}'");

        cagnotte.Annuler();

        await repository.Upsert(cagnotte);
    }
}
