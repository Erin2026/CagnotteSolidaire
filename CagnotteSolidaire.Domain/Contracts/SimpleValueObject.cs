using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CagnotteSolidaire.Domain.Contracts;
public abstract class SimpleValueObject<T>
{
    public T Value { get; }
    protected abstract T Sanitize(T raw);
    public SimpleValueObject(T value) => Value = Sanitize(value);
}

