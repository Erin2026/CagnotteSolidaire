using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Contracts;

namespace CagnotteSolidaire.Domain.ValueObjects;

public class Montant(decimal value) : SimpleValueObject<decimal>(value)
{
    protected override decimal Sanitize(decimal raw)
    {
        if (raw < 0)
            throw new ApplicationException("Le montant ne peut pas être négatif");

        return Math.Round(raw, 2);
    }

    public override string ToString() => $"{Value:F2}€";
    public static implicit operator decimal(Montant montant) => montant.Value;
    public static implicit operator Montant(decimal d) => new(d);
}

