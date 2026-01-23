using CagnotteSolidaire.Domain.Repositories;
using LibRator.Domain.Queries.Customers;
using MediatR;

namespace LibRator.Domain.Queries.Utilisateurs;

public class GetCustomerQueryHandler(IUtilisateurQueryRepository _repository) : IRequestHandler<GetUtilisateurQuery, UtilisateurDTO>
{
    public Task<UtilisateurDTO> Handle(GetUtilisateurQuery query, CancellationToken cancellationToken = default)
        => _repository.GetOne(query.Id);
}
