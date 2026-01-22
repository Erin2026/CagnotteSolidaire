using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Entities;

public class Participation
{
    public Guid Id { get; set; }

    // Montant de l'intention de don
    public decimal Montant { get; set; }

    // Date
    public DateTime DateParticipation { get; set; } = DateTime.UtcNow;

    // Relations
    public Guid CagnotteId { get; set; }
    public virtual Cagnotte Cagnotte { get; set; } = null!;

    public string ParticipantId { get; set; } = string.Empty;
    public virtual ApplicationUser Participant { get; set; } = null!;

    // Commentaire optionnel
    public string? Commentaire { get; set; }
}