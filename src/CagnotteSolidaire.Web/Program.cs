using CagnotteSolidaire.Web.Components;
using CagnotteSolidaire.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Désactiver l'antiforgery pour le développement/test
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

// HttpClient via IHttpClientFactory
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "http://localhost:5163");
});

// HttpClient par défaut pour les pages Blazor qui injectent HttpClient directement
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiClient");
});

// Services - AuthStateService en Singleton pour conserver le cache
builder.Services.AddSingleton<AuthStateService>();

// AuthService en Scoped (peut utiliser ProtectedSessionStorage)
builder.Services.AddScoped<AuthService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection(); // HTTPS seulement en production
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();



