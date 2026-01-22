using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Auth.Commands.RegisterGestionnaire;

public record RegisterGestionnaireCommand : ICommand<Result<string>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Nom { get; init; }
    public required string Prenom { get; init; }
    
    // Données de l'association (depuis API JO)
    public required string AssociationNom { get; init; }
    public required string SIREN { get; init; }
    public string? RNA { get; init; }
    public string? Adresse { get; init; }
    public string? CodePostal { get; init; }
    public string? Ville { get; init; }
    public required string Departement { get; init; }
}
