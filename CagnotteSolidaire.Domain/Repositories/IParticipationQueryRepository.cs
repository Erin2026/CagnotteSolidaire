using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Participations;

using LibRator.Domain.Queries.Participations;

namespace CagnotteSolidaire.Domain.Repositories;

public interface IParticipationQueryRepository
{
    Task<ParticipationsDTO> GetOne(int id);
    Task<ParticipationsDTO[]> GetAll(int limit, int offset, FindParticipationQuery query);
}
