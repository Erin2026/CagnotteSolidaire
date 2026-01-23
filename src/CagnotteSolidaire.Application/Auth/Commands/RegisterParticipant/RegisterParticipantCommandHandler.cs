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
        Console.WriteLine($"[REGISTER] Tentative d'inscription: {request.Email}");
        
        // Vérifier si l'email existe déjà
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            Console.WriteLine($"[REGISTER] Email déjà utilisé: {request.Email}");
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

        Console.WriteLine($"[REGISTER] Création du compte pour: {request.Email}");
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"[REGISTER] Échec création: {errors}");
            return Result<string>.Failure($"Erreur lors de la création du compte : {errors}");
        }

        Console.WriteLine($"[REGISTER] Compte créé avec succès. Ajout du rôle Participant...");
        
        // Ajouter le rôle Participant
        await _userManager.AddToRoleAsync(user, "Participant");

        Console.WriteLine($"[REGISTER] Inscription terminée avec succès pour: {request.Email} (ID: {user.Id})");
        
        return Result<string>.Success(user.Id);
    }

}
