using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Participations;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class CagnotteDTO(Cagnotte entity) : CagnottesDTO(entity)
{
    public int GestionnaireId { get; set; }
    public List<ParticipationsDTO> Participations { get; set; } = new();
}
