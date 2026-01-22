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
    Utilisateur participant,
    Cagnotte cagnotte,
    decimal montant) : Entity(id)
{
    public Utilisateur Participant { get; } = participant;
    public Cagnotte Cagnotte { get; } = cagnotte;
    public Montant Montant { get; } = montant;
}
