using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Cagnottes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UtilisateurDTO(Utilisateur entity) : UtilisateursDTO(entity)
{
    public string Email { get; set; } = entity.Email.Address;
    
    // A modifier ?
}
