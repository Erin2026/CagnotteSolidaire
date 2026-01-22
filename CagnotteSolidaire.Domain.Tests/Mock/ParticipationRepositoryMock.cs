using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Queries.Cagnottes;
using CagnotteSolidaire.Domain.Queries.Participations;
using CagnotteSolidaire.Domain.Repositories;
using LibRator.Domain.Queries.Participations;

namespace CagnotteSolidaire.Domain.Tests.Mocks;

public class ParticipationRepositoryMock(Participation[] fixture, DateTime creationDate) : IParticipationCommandRepository, IParticipationQueryRepository
{
    public Dictionary<Participation, ParticipationsDTO> Data = fixture.Select((entity, index) => new
    {
        Entity = entity,
        DTO = new ParticipationsDTO(entity)
        {
            CreationDate = creationDate,
            ModificationDate = creationDate.AddMonths(index)
        }
    })
    .ToDictionary(data => data.Entity, data => data.DTO);

    public Task<ParticipationsDTO[]> GetAll(
    int limit,
    int offset,
    FindParticipationQuery? query)
    {
        var result = Data.Keys.AsQueryable();

        if (query != null)
        {
            if (query.CagnotteId.HasValue)
                result = result.Where(p => p.Cagnotte.Id == query.CagnotteId.Value);

            if (query.ParticipantId.HasValue)
                result = result.Where(p => p.Participant.Id == query.ParticipantId.Value);

            if (query.MinMontant.HasValue)
                result = result.Where(p => p.Montant >= query.MinMontant.Value);

            if (query.MaxMontant.HasValue)
                result = result.Where(p => p.Montant <= query.MaxMontant.Value);
        }

        return Task.FromResult(result
            .Skip(offset)
            .Take(limit)
            .Select(p => (ParticipationsDTO)Data[p])
            .ToArray());
    }



    public Task<ParticipationsDTO> GetOne(int id) => Task.FromResult(Data.Values.First(entity => entity.Id == id));

    public Task<int> Upsert(Participation book)
    {
        var now = DateTime.Now;

        if (book.Id == 0)
        {
            var dto = new ParticipationsDTO(book)
            {
                Id = Data.Count + 1,
                CreationDate = now,
                ModificationDate = now
            };
            Data.Add(book, dto);
            return Task.FromResult(dto.Id);
        }
        else
        {
            var key = Data.Keys.Single(b => b.Id == book.Id);
            var updatedData = new ParticipationsDTO(book)
            {
                CreationDate = Data[key].CreationDate,
                ModificationDate = now
            };
            Data[key] = updatedData;
            return Task.FromResult(key.Id);
        }
    }

    Task<Participation?> IParticipationCommandRepository.GetOne(int id) => Task.FromResult(Data.Keys.FirstOrDefault(entity => entity.Id == id));

}