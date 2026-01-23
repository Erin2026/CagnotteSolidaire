namespace CagnotteSolidaire.Web.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public Guid? AssociationId { get; set; }  // Pour les gestionnaires
}


public class RegisterParticipantRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
}

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
