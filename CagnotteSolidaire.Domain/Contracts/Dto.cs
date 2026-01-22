using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CagnotteSolidaire.Domain.Contracts;
public abstract class DTO<T>(T entity) where T : Entity
{
    public int Id { get; set; } = entity.Id;
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
}


