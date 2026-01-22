namespace CagnotteSolidaire.Domain.Interfaces;

public interface IJOAssociationsService
{
    Task<AssociationApiSearchResult> SearchAssociationsAsync(string searchTerm, string departement = "68");
}

public class AssociationApiSearchResult
{
    public int TotalResults { get; set; }
    public List<AssociationApiRecord> Records { get; set; } = new();
}

public class AssociationApiRecord
{
    public AssociationApiFields Fields { get; set; } = new();
}

public class AssociationApiFields
{
    public string? Nom { get; set; }
    public string? SIREN { get; set; }
    public string? RNA { get; set; }
    public string? Ville { get; set; }
    public string? CodePostal { get; set; }
    public string? NumeroVoie { get; set; }
    public string? LibelleVoie { get; set; }
}
