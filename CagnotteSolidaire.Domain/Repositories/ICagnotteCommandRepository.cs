using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Repositories;

public interface ICagnotteCommandRepository
{
    Task<int> Upsert(Cagnotte cagnotte);
    Task<Cagnotte?> GetOne(int id);
}
