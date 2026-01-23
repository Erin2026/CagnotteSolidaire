using CagnotteSolidaire.Application.Common.Models;
using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Enums;
using CagnotteSolidaire.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace CagnotteSolidaire.Application.Auth.Commands.RegisterGestionnaire;

public class RegisterGestionnaireCommandHandler : IRequestHandler<RegisterGestionnaireCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAssociationRepository _associationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterGestionnaireCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAssociationRepository associationRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _associationRepository = associationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(RegisterGestionnaireCommand request, CancellationToken cancellationToken)
    {
        // Verifier si l'email existe deja
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<string>.Failure("Cet email est deja utilise");
        }

        // Verifier si l'association existe deja (par SIREN si disponible)
        Association? existingAssociation = null;
        
        if (!string.IsNullOrEmpty(request.SIREN))
        {
            existingAssociation = await _associationRepository.GetBySIRENAsync(request.SIREN);
        }
        
        Association association;
        if (existingAssociation == null)
        {
            // Creer la nouvelle association
            association = new Association
            {
                Id = Guid.NewGuid(),
                Nom = request.AssociationNom,
                SIREN = request.SIREN,  // Peut etre null
                RNA = request.RNA,
                Departement = request.Departement,
                Adresse = request.Adresse,
                CodePostal = request.CodePostal,
                Ville = request.Ville,
                DateCreation = DateTime.UtcNow
            };

            await _associationRepository.AddAsync(association);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            association = existingAssociation;
        }

        // Creer le nouvel utilisateur gestionnaire
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Nom = request.Nom,
            Prenom = request.Prenom,
            Role = UserRole.Gestionnaire,
            AssociationId = association.Id,
            DateInscription = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Erreur lors de la creation du compte : {errors}");
        }

        // Ajouter le role Gestionnaire
        await _userManager.AddToRoleAsync(user, "Gestionnaire");

        return Result<string>.Success(user.Id);
    }
}
