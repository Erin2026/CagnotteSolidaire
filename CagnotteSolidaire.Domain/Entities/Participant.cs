using CagnotteSolidaire.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CagnotteSolidaire.Domain.Entities;

public class Participant(int id, string nom, string prenom, string email)
    : Utilisateur(id, nom, prenom, email)
{
}
