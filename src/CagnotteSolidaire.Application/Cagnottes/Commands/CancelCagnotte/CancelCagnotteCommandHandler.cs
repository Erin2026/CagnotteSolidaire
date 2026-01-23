using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using MediatR;

namespace CagnotteSolidaire.Application.Cagnottes.Commands.CancelCagnotte;


public class CancelCagnotteCommandHandler : IRequestHandler<CancelCagnotteCommand, Result>
{
    private readonly ICagnotteRepository _cagnotteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CancelCagnotteCommandHandler(
        ICagnotteRepository cagnotteRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _cagnotteRepository = cagnotteRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Result> Handle(CancelCagnotteCommand request, CancellationToken cancellationToken)
    {
        // Recuperer la cagnotte avec ses participations pour envoyer les emails
        var cagnotte = await _cagnotteRepository.GetWithParticipationsAsync(request.CagnotteId);
        
        if (cagnotte == null)
        {
            return Result.Failure("Cagnotte introuvable");
        }

        // Verifier que l'utilisateur est bien le gestionnaire
        if (cagnotte.GestionnaireId != request.GestionnaireId)
        {
            return Result.Failure("Vous n'etes pas autorise a annuler cette cagnotte");
        }

        // Verifier que la cagnotte est active
        if (cagnotte.Statut != StatutCagnotte.Active)
        {
            return Result.Failure("Cette cagnotte n'est plus active");
        }

        // Annuler la cagnotte
        cagnotte.Statut = StatutCagnotte.Annulee;
        cagnotte.DateCloture = DateTime.UtcNow;

        await _cagnotteRepository.UpdateAsync(cagnotte);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Envoyer les emails aux participants
        await _emailService.SendCagnotteCancelledEmailAsync(cagnotte);

        return Result.Success();
    }
}
