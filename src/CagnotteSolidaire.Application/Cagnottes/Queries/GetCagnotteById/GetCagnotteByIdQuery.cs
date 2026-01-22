using CagnotteSolidaire.Application.Common.Interfaces;

namespace CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;

public record GetCagnotteByIdQuery : IQuery<CagnotteDto?>
{
    public required Guid Id { get; init; }
}
