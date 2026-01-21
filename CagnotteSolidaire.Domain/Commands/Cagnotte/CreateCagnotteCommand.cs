using MediatR;

namespace CagnotteSolidaire.Domain.Commands.Cagnottes;

public class CreateCagnotteCommand(
    string nom,
    string description,
    decimal objectif,
    int gestionnaireId,
    string? imageUrl)
    : IRequest<int>
{
    public string Nom { get; } = nom;
    public string Description { get; } = description;
    public decimal Objectif { get; } = objectif;
    public int GestionnaireId { get; } = gestionnaireId;
    public string? ImageUrl { get; } = imageUrl;
}
