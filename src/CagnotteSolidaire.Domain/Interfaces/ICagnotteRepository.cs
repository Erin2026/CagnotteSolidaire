using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Interfaces;

public interface ICagnotteRepository : IRepository<Cagnotte>
{
    Task<IEnumerable<Cagnotte>> GetByGestionnaireIdAsync(string gestionnaireId);
    Task<Cagnotte?> GetWithParticipationsAsync(Guid id);
    Task<IEnumerable<Cagnotte>> GetActivesAsync();
    Task<IEnumerable<Cagnotte>> GetByStatutAsync(StatutCagnotte statut);
}
