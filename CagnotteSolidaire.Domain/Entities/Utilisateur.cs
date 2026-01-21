using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.ValueObjects;

namespace CagnotteSolidaire.Domain.Entities;

public abstract class Utilisateur(int id, string nom, string prenom, string email)
    : Entity(id)
{
    public Label Nom { get; } = nom;
    public Label Prenom { get; } = prenom;
    public Mail Email { get; } = email;
}
