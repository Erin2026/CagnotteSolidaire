using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.Data;

namespace CagnotteSolidaire.Infrastructure.Repositories;

public class AssociationRepository : Repository<Association>, IAssociationRepository
{
    public AssociationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Association?> GetBySIRENAsync(string siren)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.SIREN == siren);
    }

    public async Task<Association?> GetByRNAAsync(string rna)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.RNA == rna);
    }

    public async Task<bool> ExistsBySIRENAsync(string siren)
    {
        return await _dbSet
            .AnyAsync(a => a.SIREN == siren);
    }
}