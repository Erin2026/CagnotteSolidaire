using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class CloturerCagnotteCommandHandler(
    ICagnotteCommandRepository repository)
    : IRequestHandler<CloturerCagnotteCommand>
{
    public Task Handle(CloturerCagnotteCommand cmd, CancellationToken ct = default)
    {
        var cagnotte = repository.GetById(cmd.CagnotteId, ct).Result;

        if (cagnotte == null)
            throw new ApplicationException("Cagnotte not found");

        cagnotte.Cloturer(); // Peut throw si objectif non atteint

        return repository.Upsert(cagnotte, ct)
                         .ContinueWith(_ => Unit.Value, ct);
    }
}
