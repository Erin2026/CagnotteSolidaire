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
        // Recuperer la cagnotte avec ses participations pour calculer l'objectif
        var cagnotte = await _cagnotteRepository.GetWithParticipationsAsync(request.CagnotteId);
        
        if (cagnotte == null)
        {
            return Result.Failure("Cagnotte introuvable");
        }

        // Verifier que l'utilisateur est bien le gestionnaire
        if (cagnotte.GestionnaireId != request.GestionnaireId)
        {
            return Result.Failure("Vous n'etes pas autorise a cloturer cette cagnotte");
        }

        // Verifier que la cagnotte est active
        if (cagnotte.Statut != StatutCagnotte.Active)
        {
            return Result.Failure("Cette cagnotte n'est plus active");
        }

        // Verifier que l'objectif est atteint
        if (!cagnotte.ObjectifAtteint)
        {
            return Result.Failure("L'objectif n'est pas atteint. Utilisez l'annulation si vous souhaitez fermer la cagnotte.");
        }

        // Cloturer la cagnotte
        cagnotte.Statut = StatutCagnotte.Cloturee;
        cagnotte.DateCloture = DateTime.UtcNow;

        await _cagnotteRepository.UpdateAsync(cagnotte);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Envoyer les emails aux participants
        await _emailService.SendCagnotteClosedEmailAsync(cagnotte);

        return Result.Success();
    }
}
