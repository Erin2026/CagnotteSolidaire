using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Auth.Commands.Login;

public record LoginCommand : ICommand<Result<LoginResponse>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginResponse
{
    public required string Token { get; init; }
    public required string UserId { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
    public required string NomComplet { get; init; }
    public Guid? AssociationId { get; init; }  // Pour les gestionnaires
}

