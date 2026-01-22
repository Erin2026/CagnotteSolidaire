using System.Text.Json;
using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.ExternalServices.Models;

namespace CagnotteSolidaire.Infrastructure.ExternalServices;

public class JOAssociationsService : IJOAssociationsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://journal-officiel-datadila.opendatasoft.com/api/records/1.0/search/";
    private const string Dataset = "jo_associations";

    public JOAssociationsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AssociationApiSearchResult> SearchAssociationsAsync(
        string searchTerm,
        string departement = "68")
    {
        try
        {
            // Construction de la requête
            var query = $"dataset={Dataset}" +
                       $"&q={Uri.EscapeDataString(searchTerm)}" +
                       $"&refine.adresse_code_departement={departement}" +
                       $"&rows=20";

            var url = $"{BaseUrl}?{query}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<AssociationApiResponse>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            // Mapper vers le modèle du domaine
            return new AssociationApiSearchResult
            {
                TotalResults = apiResponse?.TotalResults ?? 0,
                Records = apiResponse?.Records.Select(r => new AssociationApiRecord
                {
                    Fields = new AssociationApiFields
                    {
                        Nom = r.Fields.Nom,
                        SIREN = r.Fields.SIREN,
                        RNA = r.Fields.RNA,
                        Ville = r.Fields.Ville,
                        CodePostal = r.Fields.CodePostal,
                        NumeroVoie = r.Fields.NumeroVoie,
                        LibelleVoie = r.Fields.LibelleVoie
                    }
                }).ToList() ?? new()
            };
        }
        catch (Exception ex)
        {
            // Log l'erreur (à implémenter avec ILogger)
            throw new Exception($"Erreur lors de la recherche d'associations : {ex.Message}", ex);
        }
    }
}
