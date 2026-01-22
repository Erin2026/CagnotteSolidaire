using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Queries.Participations;

public class ParticipationsDTO(Participation entity) : DTO<Participation>(entity)
{
    public int ParticipantId { get; set; }
    public int CagnotteId { get; set; }
    public decimal Montant { get; set; }
    public string? ParticipantEmail { get; set; }
}
