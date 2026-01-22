namespace CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;

public class CagnotteDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ObjectifFinancier { get; set; }
    public decimal MontantActuel { get; set; }
    public string? ImageUrl { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string NomAssociation { get; set; } = string.Empty;
    public int NombreParticipants { get; set; }
}
