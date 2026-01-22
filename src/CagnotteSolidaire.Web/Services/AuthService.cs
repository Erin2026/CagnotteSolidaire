using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CagnotteSolidaire.Web.Models;

namespace CagnotteSolidaire.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    private const string UserKey = "currentUser";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse != null)
                {
                    await _localStorage.SetItemAsync(TokenKey, loginResponse.Token);
                    await _localStorage.SetItemAsync(UserKey, loginResponse);
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.Token);
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterParticipantAsync(RegisterParticipantRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register/participant", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<LoginResponse?> GetCurrentUserAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<LoginResponse>(UserKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
