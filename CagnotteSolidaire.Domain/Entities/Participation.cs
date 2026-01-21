using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.ValueObjects;

namespace CagnotteSolidaire.Domain.Entities;

public class Participation(
    int id,
    int participantId,
    int cagnotteId,
    decimal montant) : Entity(id)
{
    public int ParticipantId { get; } = participantId;
    public int CagnotteId { get; } = cagnotteId;
    public Montant Montant { get; } = montant;
}
