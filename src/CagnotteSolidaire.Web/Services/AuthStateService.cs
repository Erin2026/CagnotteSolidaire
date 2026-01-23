using CagnotteSolidaire.Web.Models;

namespace CagnotteSolidaire.Web.Services;

/// <summary>
/// Service Singleton qui garde l'état d'authentification en mémoire
/// </summary>
public class AuthStateService
{
    private LoginResponse? _currentUser;
    private string? _currentToken;

    public LoginResponse? CurrentUser
    {
        get => _currentUser;
        set => _currentUser = value;
    }

    public string? CurrentToken
    {
        get => _currentToken;
        set => _currentToken = value;
    }

    public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_currentToken);

    public void Clear()
    {
        _currentUser = null;
        _currentToken = null;
    }
}
