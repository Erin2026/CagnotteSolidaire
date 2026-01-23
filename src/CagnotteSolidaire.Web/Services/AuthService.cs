using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CagnotteSolidaire.Web.Models;

namespace CagnotteSolidaire.Web.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly AuthStateService _authState;
    private const string TokenKey = "authToken";
    private const string UserKey = "currentUser";

    public AuthService(IHttpClientFactory httpClientFactory, ProtectedSessionStorage sessionStorage, AuthStateService authState)
    {
        _httpClientFactory = httpClientFactory;
        _sessionStorage = sessionStorage;
        _authState = authState;
    }



    public async Task<(bool success, string? role)> LoginAsync(string email, string password)
    {
        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        
        try
        {
            Console.WriteLine($"[BLAZOR] Tentative login: {email}");
            var request = new LoginRequest { Email = email, Password = password };
            var response = await httpClient.PostAsJsonAsync("api/Auth/login", request);


            Console.WriteLine($"[BLAZOR] Réponse API: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[BLAZOR] Contenu réponse: {content}");
                
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse != null)
                {
                    Console.WriteLine($"[BLAZOR] Token reçu, sauvegarde...");
                    
                    // Stocker dans le state singleton
                    _authState.CurrentUser = loginResponse;
                    _authState.CurrentToken = loginResponse.Token;
                    
                    try
                    {
                        // Essayer de sauvegarder dans le storage (peut échouer si prerendering)
                        await _sessionStorage.SetAsync(TokenKey, loginResponse.Token);
                        await _sessionStorage.SetAsync(UserKey, loginResponse);
                        Console.WriteLine($"[BLAZOR] Sauvegarde storage réussie");
                    }
                    catch (InvalidOperationException)
                    {
                        // Prerendering en cours, les données restent dans AuthState
                        Console.WriteLine($"[BLAZOR] Prerendering détecté, utilisation du cache mémoire");
                    }
                    
                    Console.WriteLine($"[BLAZOR] Login réussi pour {email}, rôle: {loginResponse.Role}");
                    return (true, loginResponse.Role);
                }


                else
                {
                    Console.WriteLine($"[BLAZOR] loginResponse est null");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[BLAZOR] Erreur login: {response.StatusCode} - {errorContent}");
            }
            return (false, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BLAZOR] Exception login: {ex.Message}");
            Console.WriteLine($"[BLAZOR] StackTrace: {ex.StackTrace}");
            return (false, null);
        }
    }




    public async Task<bool> RegisterParticipantAsync(RegisterParticipantRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient("ApiClient");
        
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/Auth/register/participant", request);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        _authState.Clear();
        
        try
        {
            await _sessionStorage.DeleteAsync(TokenKey);
            await _sessionStorage.DeleteAsync(UserKey);
        }
        catch
        {
            // Ignorer les erreurs de storage
        }
    }


    public async Task<LoginResponse?> GetCurrentUserAsync()
    {
        // D'abord vérifier le state singleton
        if (_authState.CurrentUser != null)
        {
            return _authState.CurrentUser;
        }
        
        try
        {
            var result = await _sessionStorage.GetAsync<LoginResponse>(UserKey);
            if (result.Success && result.Value != null)
            {
                _authState.CurrentUser = result.Value;
                return result.Value;
            }
        }
        catch
        {
            // Ignorer les erreurs de storage
        }
        
        return null;
    }

    public async Task<string?> GetTokenAsync()
    {
        // D'abord vérifier le state singleton
        if (_authState.CurrentToken != null)
        {
            return _authState.CurrentToken;
        }
        
        try
        {
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            if (result.Success && result.Value != null)
            {
                _authState.CurrentToken = result.Value;
                return result.Value;
            }
        }
        catch
        {
            // Ignorer les erreurs de storage
        }
        
        return null;
    }



    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}

