using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Repositories;

public interface IParticipationCommandRepository
{
    Task<int> Create(Participation participation, CancellationToken ct);
}
