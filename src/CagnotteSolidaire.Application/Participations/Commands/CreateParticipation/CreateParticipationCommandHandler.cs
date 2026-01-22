using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Participations.Commands.CreateParticipation;

public class CreateParticipationCommandHandler : IRequestHandler<CreateParticipationCommand, Result<Guid>>
{
    private readonly IParticipationRepository _participationRepository;
    private readonly ICagnotteRepository _cagnotteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateParticipationCommandHandler(
        IParticipationRepository participationRepository,
        ICagnotteRepository cagnotteRepository,
        IUnitOfWork unitOfWork)
    {
        _participationRepository = participationRepository;
        _cagnotteRepository = cagnotteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateParticipationCommand request, CancellationToken cancellationToken)
    {
        // Vérifier que la cagnotte existe et est active
        var cagnotte = await _cagnotteRepository.GetByIdAsync(request.CagnotteId);
        if (cagnotte == null)
        {
            return Result<Guid>.Failure("Cagnotte introuvable");
        }

        if (cagnotte.Statut != StatutCagnotte.Active)
        {
            return Result<Guid>.Failure("Cette cagnotte n'est plus active");
        }

        // Validation du montant
        if (request.Montant <= 0)
        {
            return Result<Guid>.Failure("Le montant doit être supérieur à 0");
        }

        // Créer la participation
        var participation = new Participation
        {
            Id = Guid.NewGuid(),
            CagnotteId = request.CagnotteId,
            ParticipantId = request.ParticipantId,
            Montant = request.Montant,
            Commentaire = request.Commentaire,
            DateParticipation = DateTime.UtcNow
        };

        await _participationRepository.AddAsync(participation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(participation.Id);
    }
}
