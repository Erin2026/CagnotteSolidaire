namespace CagnotteSolidaire.Application.Participations.Queries.GetParticipantsByCagnotte;

public class ParticipantDto
{
    public Guid Id { get; set; }
    public string NomComplet { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateParticipation { get; set; }
}
