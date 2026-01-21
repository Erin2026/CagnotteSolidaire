using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class CloturerCagnotteCommand(int cagnotteId)
    : IRequest
{
    public int CagnotteId { get; } = cagnotteId;
}
