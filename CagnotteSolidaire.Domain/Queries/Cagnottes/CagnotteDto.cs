using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class CagnotteDTO : CagnottesDTO
{
    public CagnotteDTO() { }
    public CagnotteDTO(Cagnotte entity) : base(entity)
    {
        GestionnaireId = entity.GestionnaireId;
        // Les participations seront ajoutées par le repository
    }

    public int GestionnaireId { get; set; }
    public List<ParticipationDTO> Participations { get; set; } = new();
}
