using CagnotteSolidaire.Domain.Entities;

namespace CagnotteSolidaire.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}

