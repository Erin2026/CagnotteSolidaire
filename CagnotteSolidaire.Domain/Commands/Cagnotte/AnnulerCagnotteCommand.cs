using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class AnnulerCagnotteCommand(int cagnotteId)
    : IRequest
{
    public int CagnotteId { get; } = cagnotteId;
}
