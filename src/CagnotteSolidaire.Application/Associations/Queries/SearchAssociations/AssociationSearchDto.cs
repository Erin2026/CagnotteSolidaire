namespace CagnotteSolidaire.Application.Associations.Queries.SearchAssociations;

public class AssociationSearchDto
{
    public string Nom { get; set; } = string.Empty;
    public string? SIREN { get; set; }
    public string? RNA { get; set; }
    public string? Ville { get; set; }
    public string? CodePostal { get; set; }
    public string? Adresse { get; set; }
}
