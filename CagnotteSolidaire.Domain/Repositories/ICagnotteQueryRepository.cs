using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Queries.Cagnottes;

namespace CagnotteSolidaire.Domain.Repositories;

public interface ICagnotteQueryRepository
{
    Task<List<CagnottesDTO>> GetByGestionnaire(int gestionnaireId);
    Task<CagnotteDTO> GetOne(int id);
}
