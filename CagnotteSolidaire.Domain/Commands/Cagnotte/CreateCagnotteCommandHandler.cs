using CagnotteSolidaire.Domain.Commands.Cagnottes;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Repositories;
using MediatR;

public class CreateCagnotteCommandHandler(
    ICagnotteCommandRepository repository)
    : IRequestHandler<CreateCagnotteCommand, int>
{
    public Task<int> Handle(CreateCagnotteCommand cmd, CancellationToken ct) =>
        repository.Upsert(
            new Cagnotte(
                0,
                cmd.Nom,
                cmd.Description,
                cmd.Objectif,
                cmd.GestionnaireId,
                cmd.ImageUrl // maintenant dans le constructeur
            ),
            ct
        );
}
