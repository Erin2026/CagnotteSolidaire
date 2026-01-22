using CagnotteSolidaire.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Interfaces;

public interface IParticipationRepository : IRepository<Participation>
{
    Task<IEnumerable<Participation>> GetByCagnotteIdAsync(Guid cagnotteId);
    Task<IEnumerable<Participation>> GetByParticipantIdAsync(string participantId);
    Task<bool> UserHasParticipatedAsync(Guid cagnotteId, string participantId);
    Task<decimal> GetTotalByCagnotteAsync(Guid cagnotteId);
}