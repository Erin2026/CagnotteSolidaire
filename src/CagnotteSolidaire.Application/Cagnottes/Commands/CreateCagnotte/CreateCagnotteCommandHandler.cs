using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Cagnottes.Commands.CreateCagnotte;

public class CreateCagnotteCommandHandler : IRequestHandler<CreateCagnotteCommand, Result<Guid>>
{
    private readonly ICagnotteRepository _cagnotteRepository;
    private readonly IAssociationRepository _associationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCagnotteCommandHandler(
        ICagnotteRepository cagnotteRepository,
        IAssociationRepository associationRepository,
        IUnitOfWork unitOfWork)
    {
        _cagnotteRepository = cagnotteRepository;
        _associationRepository = associationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCagnotteCommand request, CancellationToken cancellationToken)
    {
        // Vérifier que l'association existe
        var association = await _associationRepository.GetByIdAsync(request.AssociationId);
        if (association == null)
        {
            return Result<Guid>.Failure("Association introuvable");
        }

        // TODO: Vérifier que l'utilisateur courant est bien gestionnaire de cette association

        // Créer la nouvelle cagnotte
        var cagnotte = new Cagnotte
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Description = request.Description,
            ObjectifFinancier = request.ObjectifFinancier,
            ImageUrl = request.ImageUrl,
            GestionnaireId = request.GestionnaireId,
            DateCreation = DateTime.UtcNow,
            Statut = StatutCagnotte.Active
        };

        await _cagnotteRepository.AddAsync(cagnotte);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(cagnotte.Id);
    }
}

