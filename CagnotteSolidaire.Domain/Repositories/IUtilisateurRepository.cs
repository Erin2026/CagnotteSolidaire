using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Repositories;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByEmail(string email, CancellationToken ct);
}
