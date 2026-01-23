using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Entities;

public class Association
{
    public Guid Id { get; set; }

    // Donnees de l'API JO
    public string Nom { get; set; } = string.Empty;
    public string? SIREN { get; set; }  // Nullable car certaines associations n'ont pas de SIREN
    public string? RNA { get; set; }
    public string Departement { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? CodePostal { get; set; }
    public string? Ville { get; set; }


    // Dates
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    // Relations
    public virtual ICollection<ApplicationUser> Gestionnaires { get; set; } = new List<ApplicationUser>();
}

