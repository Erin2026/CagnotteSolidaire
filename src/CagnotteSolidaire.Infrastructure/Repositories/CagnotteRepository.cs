using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.Data;

namespace CagnotteSolidaire.Infrastructure.Repositories;

public class CagnotteRepository : Repository<Cagnotte>, ICagnotteRepository
{
    public CagnotteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cagnotte>> GetByGestionnaireIdAsync(string gestionnaireId)
    {
        return await _dbSet
            .Include(c => c.Participations)
            .Where(c => c.GestionnaireId == gestionnaireId)
            .OrderByDescending(c => c.DateCreation)
            .ToListAsync();
    }

    public async Task<Cagnotte?> GetWithParticipationsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Participations)
                .ThenInclude(p => p.Participant)
            .Include(c => c.Gestionnaire)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Cagnotte>> GetActivesAsync()
    {
        return await _dbSet
            .Include(c => c.Participations)
            .Where(c => c.Statut == StatutCagnotte.Active)
            .OrderByDescending(c => c.DateCreation)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cagnotte>> GetByStatutAsync(StatutCagnotte statut)
    {
        return await _dbSet
            .Include(c => c.Participations)
            .Where(c => c.Statut == statut)
            .OrderByDescending(c => c.DateCreation)
            .ToListAsync();
    }
}
