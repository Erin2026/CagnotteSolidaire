using CagnotteSolidaire.Domain.Entities;
using CagnotteSolidaire.Domain.Repositories;

namespace CagnotteSolidaire.Domain.Tests.Mocks;

public class UtilisateurRepositoryMock : IUtilisateurQueryRepository
{
    private readonly Dictionary<string, Utilisateur> _utilisateurs = new();

    public Task<Utilisateur?> GetByEmail(string email, CancellationToken ct)
    {
        _utilisateurs.TryGetValue(email.ToLower(), out var utilisateur);
        return Task.FromResult(utilisateur);
    }

    // Méthodes utilitaires pour les tests
    public void AddUtilisateur(Utilisateur utilisateur)
    {
        _utilisateurs[utilisateur.Email.Value.ToLower()] = utilisateur;
    }

    public void Clear()
    {
        _utilisateurs.Clear();
    }
}