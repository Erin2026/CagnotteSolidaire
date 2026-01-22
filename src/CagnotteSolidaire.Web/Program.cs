using CagnotteSolidaire.Web.Components;
using CagnotteSolidaire.Web.Services;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// HttpClient configuré pour l'API
builder.Services.AddScoped<AuthenticationMessageHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthenticationMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "http://localhost:5163")
    };
    return httpClient;
});

// Services
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

