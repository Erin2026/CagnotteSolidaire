using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CagnotteSolidaire.Infrastructure.Services;


public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtTokenGenerator(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "CagnotteSolidaireAPI";
        var audience = jwtSettings["Audience"] ?? "CagnotteSolidaireClient";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Récupérer les rôles de l'utilisateur
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.NomComplet),
            new Claim("nom", user.Nom),
            new Claim("prenom", user.Prenom),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Ajouter les rôles comme claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Ajouter l'AssociationId pour les gestionnaires
        if (user.AssociationId.HasValue)
        {
            claims.Add(new Claim("associationId", user.AssociationId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
