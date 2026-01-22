using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity;
using CagnotteSolidaire.Domain.Enums;

namespace CagnotteSolidaire.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    // Informations personnelles
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;

    // Rôle
    public UserRole Role { get; set; }

    // Association (uniquement pour les Gestionnaires)
    public Guid? AssociationId { get; set; }
    public virtual Association? Association { get; set; }

    // Relations
    public virtual ICollection<Cagnotte> CagnottesGerees { get; set; } = new List<Cagnotte>();
    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();

    // Dates
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;

    // Propriété calculée
    public string NomComplet => $"{Prenom} {Nom}";
}
