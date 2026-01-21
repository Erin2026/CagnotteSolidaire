using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CagnotteSolidaire.Domain.Contracts;
public abstract class Entity(int id)
{
    public int Id { get; } = id;

}
