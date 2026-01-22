using System;
using System.Collections.Generic;
using System.Text;

using CagnotteSolidaire.Domain.Enums;

namespace CagnotteSolidaire.Domain.Entities;

public class Cagnotte
{
    public Guid Id { get; set; }

    // Informations de base
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ObjectifFinancier { get; set; }
    public string? ImageUrl { get; set; }

    // Statut
    public StatutCagnotte Statut { get; set; } = StatutCagnotte.Active;

    // Dates
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateCloture { get; set; }

    // Gestionnaire (Association)
    public string GestionnaireId { get; set; } = string.Empty;
    public virtual ApplicationUser Gestionnaire { get; set; } = null!;

    // Relations
    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    // Propriétés calculées
    public decimal MontantCollecte => Participations?.Sum(p => p.Montant) ?? 0;
    public decimal PourcentageAtteint => ObjectifFinancier > 0
        ? Math.Round((MontantCollecte / ObjectifFinancier) * 100, 2)
        : 0;
    public bool ObjectifAtteint => MontantCollecte >= ObjectifFinancier;
    public int NombreParticipants => Participations?.Count ?? 0;
}
