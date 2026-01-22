using CagnotteSolidaire.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CagnotteSolidaire.Domain.Interfaces;

public interface IAssociationRepository : IRepository<Association>
{
    Task<Association?> GetBySIRENAsync(string siren);
    Task<Association?> GetByRNAAsync(string rna);
    Task<bool> ExistsBySIRENAsync(string siren);
}
