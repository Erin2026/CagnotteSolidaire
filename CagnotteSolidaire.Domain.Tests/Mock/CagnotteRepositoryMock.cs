using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Cagnottes;
using CagnotteSolidaire.Domain.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CagnotteSolidaire.Domain.Tests.Mocks;

public class CagnotteRepositoryMock(Cagnotte[] fixture, DateTime creationDate) : ICagnotteCommandRepository, ICagnotteQueryRepository
{
    // Dictionnaire pour stocker les cagnottes en mémoire
    public  Dictionary<Cagnotte, CagnotteDTO> Data = fixture.Select((entity, index)=> new
    {
        Entity =entity,
        DTO = new CagnotteDTO(entity)
        {
            CreationDate = creationDate,
            ModificationDate = creationDate.AddMonths(index)
        }
    })
    .ToDictionary(data => data.Entity, data => data.DTO);

    public Task<int> Upsert(Cagnotte cagnotte)
    {
        var now = DateTime.Now;

        if (cagnotte.Id == 0)
        {
            var dto = new CagnotteDTO(cagnotte)
            {
                Id = Data.Count + 1,
                CreationDate = now,
                ModificationDate = now
            };
            Data.Add(cagnotte, dto);
            return Task.FromResult(dto.Id);
        }
        else
        {
            var key = Data.Keys.Single(b => b.Id == cagnotte.Id);
            var updatedData = new CagnotteDTO(cagnotte)
            {
                CreationDate = Data[key].CreationDate,
                ModificationDate = now
            };
            Data[key] = updatedData;
            return Task.FromResult(key.Id);
        }
    }

    public Task<CagnotteDTO> GetOne(int id) => Task.FromResult(Data.Values.First(entity => entity.Id == id));


    public Task<List<CagnottesDTO>> GetByGestionnaire(int gestionnaireId)
    {
        var cagnottes = Data.Keys
            .Where(c => c.GestionnaireId == gestionnaireId)
            .Select(c =>new CagnottesDTO(c))
            .ToList();

        return Task.FromResult(cagnottes);
    }

    Task<Cagnotte?> ICagnotteCommandRepository.GetOne(int id) => Task.FromResult(Data.Keys.FirstOrDefault(entity => entity.Id == id));

}