using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Repositories;

public interface IUtilisateurQueryRepository
{
    Task<Utilisateur?> GetByEmail(string email, CancellationToken ct = default);

    Task<Utilisateur?> GetOne(int userId, CancellationToken ct = default);

    Task<Utilisateur?> GetAll(int limit, CancellationToken ct = default);


}
