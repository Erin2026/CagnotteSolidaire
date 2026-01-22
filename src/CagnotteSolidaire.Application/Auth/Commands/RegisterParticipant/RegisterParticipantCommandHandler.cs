using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace CagnotteSolidaire.Application.Auth.Commands.RegisterParticipant;

public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterParticipantCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<string>> Handle(RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        // Vérifier si l'email existe déjà
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<string>.Failure("Cet email est déjà utilisé");
        }

        // Créer le nouvel utilisateur
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Nom = request.Nom,
            Prenom = request.Prenom,
            Role = UserRole.Participant,
            DateInscription = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Erreur lors de la création du compte : {errors}");
        }

        // Ajouter le rôle Participant
        await _userManager.AddToRoleAsync(user, "Participant");

        return Result<string>.Success(user.Id);
    }
}
