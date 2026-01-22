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
        // Trouver l'utilisateur
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<LoginResponse>.Failure("Email ou mot de passe incorrect");
        }

        // Vérifier le mot de passe
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!passwordValid)
        {
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
            NomComplet = user.NomComplet
        };

        return Result<LoginResponse>.Success(response);
    }
}


