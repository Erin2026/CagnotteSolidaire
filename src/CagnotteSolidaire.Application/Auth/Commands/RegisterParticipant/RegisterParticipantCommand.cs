using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;

namespace CagnotteSolidaire.Application.Auth.Commands.RegisterParticipant;

public record RegisterParticipantCommand : ICommand<Result<string>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Nom { get; init; }
    public required string Prenom { get; init; }
}
