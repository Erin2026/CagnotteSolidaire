using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Repositories;

public interface IUtilisateurCommandRepository
{
    Task<int> Upsert(Utilisateur customer);

    Task<Utilisateur?> GetByEmail(string email, CancellationToken ct = default);

    Task<Utilisateur?> GetOne(int userId, CancellationToken ct = default);

}
