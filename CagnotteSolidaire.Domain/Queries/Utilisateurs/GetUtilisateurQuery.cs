using MediatR;

namespace LibRator.Domain.Queries.Customers;

public class GetUtilisateurQuery(int id): IRequest<UtilisateurDTO>
{
    public int Id { get; } = id;
}
