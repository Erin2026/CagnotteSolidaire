using CagnotteSolidaire.Application.Common.Interfaces;
using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace CagnotteSolidaire.Application.Auth.Commands.Login;


public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // DEBUG: Log de la tentative
        Console.WriteLine($"[LOGIN] Tentative de connexion pour: {request.Email}");
        
        // Trouver l'utilisateur
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            Console.WriteLine($"[LOGIN] Utilisateur introuvable: {request.Email}");
            return Result<LoginResponse>.Failure("Email ou mot de passe incorrect");
        }

        Console.WriteLine($"[LOGIN] Utilisateur trouvé: {user.Email}, Vérification du mot de passe...");

        // Vérifier le mot de passe
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!passwordValid)
        {
            Console.WriteLine($"[LOGIN] Mot de passe incorrect pour: {request.Email}");
            // Incrémenter le compteur d'échecs
            await _userManager.AccessFailedAsync(user);

            
            if (await _userManager.IsLockedOutAsync(user))
            {
                return Result<LoginResponse>.Failure("Compte verrouillé. Réessayez plus tard.");
            }
            
            return Result<LoginResponse>.Failure("Email ou mot de passe incorrect");
        }

        // Réinitialiser le compteur d'échecs
        await _userManager.ResetAccessFailedCountAsync(user);

        // Générer le token JWT
        var token = await _jwtTokenGenerator.GenerateTokenAsync(user);

        var response = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            Role = user.Role.ToString(),
            NomComplet = user.NomComplet,
            AssociationId = user.AssociationId  // Pour les gestionnaires
        };

        return Result<LoginResponse>.Success(response);
    }
}



