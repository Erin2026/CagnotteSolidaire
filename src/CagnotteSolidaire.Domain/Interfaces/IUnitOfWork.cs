using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAssociationRepository Associations { get; }
    ICagnotteRepository Cagnottes { get; }
    IParticipationRepository Participations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
