using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Repositories;
using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class CloturerCagnotteCommandHandler(
    ICagnotteCommandRepository _repository)
    : IRequestHandler<CloturerCagnotteCommand>
{
    public async Task Handle(CloturerCagnotteCommand command, CancellationToken cancellationToken = default)
    {
        Cagnotte cagnotte = await _repository.GetOne(command.CagnotteId)
            ?? throw new ApplicationException($"Cannot find cagnotte with id '{command.CagnotteId}'");

        var cloturer = cagnotte.Cloturer();
    }
}
