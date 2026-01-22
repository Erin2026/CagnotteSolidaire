using System;
using System.Collections.Generic;
using System.Text;

using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.Data;

namespace CagnotteSolidaire.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IAssociationRepository? _associations;
    private ICagnotteRepository? _cagnottes;
    private IParticipationRepository? _participations;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IAssociationRepository Associations =>
        _associations ??= new AssociationRepository(_context);

    public ICagnotteRepository Cagnottes =>
        _cagnottes ??= new CagnotteRepository(_context);

    public IParticipationRepository Participations =>
        _participations ??= new ParticipationRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
