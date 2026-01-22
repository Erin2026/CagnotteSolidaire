using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Enums;

public enum StatutCagnotte
{
    Active = 1,
    Cloturee = 2,  // Objectif atteint
    Annulee = 3    // Objectif non atteint
}
