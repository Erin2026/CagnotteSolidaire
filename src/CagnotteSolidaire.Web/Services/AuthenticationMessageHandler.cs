using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace CagnotteSolidaire.Web.Services;

public class AuthenticationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthenticationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

