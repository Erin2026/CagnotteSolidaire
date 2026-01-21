using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class CagnottesDTO : CreateCagnotteCommand<Cagnotte>
{
    public CagnottesDTO() : base(null!) { }
    public CagnottesDTO(Cagnotte entity) : base(entity)
    {
        Nom = entity.Nom;
        Description = entity.Description;
        Objectif = entity.Objectif.Value;
        MontantActuel = entity.MontantTotal;
        Statut = entity.Statut.ToString();
        ImageUrl = entity.ImageUrl;
    }

    public string? Nom { get; set; }
    public string? Description { get; set; }
    public decimal Objectif { get; set; }
    public decimal MontantActuel { get; set; }
    public string? Statut { get; set; }
    public string? ImageUrl { get; set; }
}
