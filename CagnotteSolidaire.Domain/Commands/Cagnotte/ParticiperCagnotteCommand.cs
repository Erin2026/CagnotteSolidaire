using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class ParticiperCagnotteCommand(
    int cagnotteId,
    int participantId,
    decimal montant)
    : IRequest<int>
{
    public int CagnotteId { get; } = cagnotteId;
    public int ParticipantId { get; } = participantId;
    public decimal Montant { get; } = montant;
}
