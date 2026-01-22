using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Cagnottes.Commands.CloseCagnotte;


public class CloseCagnotteCommandHandler : IRequestHandler<CloseCagnotteCommand, Result>
{
    private readonly ICagnotteRepository _cagnotteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CloseCagnotteCommandHandler(
        ICagnotteRepository cagnotteRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _cagnotteRepository = cagnotteRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result> Handle(CloseCagnotteCommand request, CancellationToken cancellationToken)
    {
        // Récupérer la cagnotte avec ses participations
        var cagnotte = await _cagnotteRepository.GetByIdAsync(request.CagnotteId);
        
        if (cagnotte == null)
        {
            return Result.Failure("Cagnotte introuvable");
        }

        // Vérifier que l'utilisateur est bien le gestionnaire
        if (cagnotte.GestionnaireId != request.GestionnaireId)
        {
            return Result.Failure("Vous n'êtes pas autorisé à clôturer cette cagnotte");
        }

        // Vérifier que la cagnotte est active
        if (cagnotte.Statut != StatutCagnotte.Active)
        {
            return Result.Failure("Cette cagnotte n'est plus active");
        }

        // Vérifier que l'objectif est atteint
        if (!cagnotte.ObjectifAtteint)
        {
            return Result.Failure("L'objectif n'est pas atteint. Utilisez l'annulation si vous souhaitez fermer la cagnotte.");
        }

        // Clôturer la cagnotte
        cagnotte.Statut = StatutCagnotte.Cloturee;
        cagnotte.DateCloture = DateTime.UtcNow;

        await _cagnotteRepository.UpdateAsync(cagnotte);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Envoyer les emails aux participants
        await _emailService.SendCagnotteClosedEmailAsync(cagnotte);

        return Result.Success();
    }
}
