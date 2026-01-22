using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CagnotteSolidaire.Domain.Entities;

public abstract class Utilisateur(int id, string nom, string prenom, string email)
    : Entity(id)
{
    public Label Nom { get; } = nom;
    public Label Prenom { get; } = prenom;
    public MailAddress Email { get; } = new MailAddress(email);
    public Role Role { get; private set; } = Role.None;
}
