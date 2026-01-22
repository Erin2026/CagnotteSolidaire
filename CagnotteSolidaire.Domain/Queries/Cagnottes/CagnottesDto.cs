using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class CagnottesDTO(Cagnotte entity) : DTO<Cagnotte>(entity)
{
    public string? Nom { get; set; }
    public string? Description { get; set; }
    public decimal Objectif { get; set; }
    public decimal MontantActuel { get; set; }
    public string? Statut { get; set; }
    public string? ImageUrl { get; set; }
}
