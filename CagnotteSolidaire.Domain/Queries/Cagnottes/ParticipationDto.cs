using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class ParticipationDTO : CreateCagnotteCommand<Participation>
{
    public ParticipationDTO() : base(null!) { }
    public ParticipationDTO(Participation entity) : base(entity)
    {
        ParticipantId = entity.ParticipantId;
        CagnotteId = entity.CagnotteId;
        Montant = entity.Montant.Value;
    }

    public int ParticipantId { get; set; }
    public int CagnotteId { get; set; }
    public decimal Montant { get; set; }
    public string? ParticipantNom { get; set; }
    public string? ParticipantEmail { get; set; }
}
