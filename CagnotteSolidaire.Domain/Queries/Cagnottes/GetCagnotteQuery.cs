using MediatR;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class GetCagnotteQuery(int cagnotteId) : IRequest<CagnotteDTO>
{
    public int CagnotteId { get; } = cagnotteId;
}
