using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Contracts;

namespace CagnotteSolidaire.Domain.ValueObjects;

public class Label(string value) : SimpleValueObject<string>(value)
{
    public const int MIN_LENGTH = 3;
    public const int MAX_LENGTH = 300;

    protected override string Sanitize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ApplicationException("Cannot be empty");

        var trimmedValue = raw.Trim();

        if (trimmedValue.Length < MIN_LENGTH)
            throw new ApplicationException("Must be at least 3 characters");

        if (trimmedValue.Length > MAX_LENGTH)
            throw new ApplicationException("Must be under 300 characters");

        return trimmedValue;
    }

    public override string ToString() => Value;
    public static implicit operator string(Label label) => label.Value;
    public static implicit operator Label(string s) => new(s);
}

