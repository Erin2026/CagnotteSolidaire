using CagnotteSolidaire.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CagnotteSolidaire.Domain.Entities;

public class Gestionnaire(
    int id,
    string nom,
    string prenom,
    string email,
    string associationRNA,
    string associationNom)
    : Utilisateur(id, nom, prenom, email)
{
    public string AssociationRNA { get; } = associationRNA;
    public string AssociationNom { get; } = associationNom;
}

