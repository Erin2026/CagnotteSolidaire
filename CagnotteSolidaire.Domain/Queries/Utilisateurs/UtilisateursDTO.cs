using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UtilisateursDTO(Utilisateur entity) : DTO<Utilisateur>(entity)
{
    public string Name { get; } = $"{entity.Nom} {entity.Prenom}";
    public string Role { get; set; } = entity.Role.ToString();
}
