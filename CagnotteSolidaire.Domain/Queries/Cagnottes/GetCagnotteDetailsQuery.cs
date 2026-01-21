using MediatR;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class GetCagnotteDetailsQuery(int cagnotteId) : IRequest<CagnotteDTO>
{
    public int CagnotteId { get; } = cagnotteId;
}
