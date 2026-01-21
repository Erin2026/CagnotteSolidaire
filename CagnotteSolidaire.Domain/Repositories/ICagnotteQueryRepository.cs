using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Queries.Cagnottes;

namespace CagnotteSolidaire.Domain.Repositories;

public interface ICagnotteQueryRepository
{
    Task<List<CagnottesDTO>> GetByGestionnaire(int gestionnaireId, CancellationToken ct);
    Task<CagnotteDTO?> GetById(int id, CancellationToken ct);
    Task<CagnotteDTO> GetOne(int cagnotteId, CancellationToken cancellationToken);
}
