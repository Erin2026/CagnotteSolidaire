using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.Data;

namespace CagnotteSolidaire.Infrastructure.Repositories;

public class ParticipationRepository : Repository<Participation>, IParticipationRepository
{
    public ParticipationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Participation>> GetByCagnotteIdAsync(Guid cagnotteId)
    {
        return await _dbSet
            .Include(p => p.Participant)
            .Where(p => p.CagnotteId == cagnotteId)
            .OrderByDescending(p => p.DateParticipation)
            .ToListAsync();
    }

    public async Task<IEnumerable<Participation>> GetByParticipantIdAsync(string participantId)
    {
        return await _dbSet
            .Include(p => p.Cagnotte)
            .Where(p => p.ParticipantId == participantId)
            .OrderByDescending(p => p.DateParticipation)
            .ToListAsync();
    }

    public async Task<bool> UserHasParticipatedAsync(Guid cagnotteId, string participantId)
    {
        return await _dbSet
            .AnyAsync(p => p.CagnotteId == cagnotteId && p.ParticipantId == participantId);
    }

    public async Task<decimal> GetTotalByCagnotteAsync(Guid cagnotteId)
    {
        return await _dbSet
            .Where(p => p.CagnotteId == cagnotteId)
            .SumAsync(p => p.Montant);
    }
}
