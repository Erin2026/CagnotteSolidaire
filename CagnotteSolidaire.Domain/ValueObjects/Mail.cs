using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Contracts;

namespace CagnotteSolidaire.Domain.ValueObjects;

public class Mail(string value) : SimpleValueObject<string>(value)
{
    protected override string Sanitize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ApplicationException("Mail cannot be empty");

        var trimmed = raw.Trim().ToLower();

        if (!trimmed.Contains("@") || !trimmed.Contains("."))
            throw new ApplicationException("Invalid Mail format");

        return trimmed;
    }

    public override string ToString() => Value;
    public static implicit operator string(Mail Mail) => Mail.Value;
    public static implicit operator Mail(string s) => new(s);
}

