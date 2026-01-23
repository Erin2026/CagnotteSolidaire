using System.Text.Json;
using CagnotteSolidaire.Domain.Interfaces;
using CagnotteSolidaire.Infrastructure.ExternalServices.Models;

namespace CagnotteSolidaire.Infrastructure.ExternalServices;

public class JOAssociationsService : IJOAssociationsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://journal-officiel-datadila.opendatasoft.com/api/explore/v2.1/catalog/datasets/jo_associations/records";

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
            // Échapper les guillemets et caractères spéciaux dans le terme de recherche
            var escapedTerm = searchTerm.Replace("\"", "\\\"").Replace("'", "\\'");
            
            // Construction de la clause WHERE qui cherche dans titre (nom), numero_rna ET dca_siren
            var whereClause = $"titre LIKE \"%{escapedTerm}%\" OR numero_rna LIKE \"%{escapedTerm}%\" OR dca_siren LIKE \"%{escapedTerm}%\"";
            
            // Construction de la requête
            var query = $"select=*" +
                       $"&where={Uri.EscapeDataString(whereClause)}" +
                       $"&limit=50"; // Nombre de résultats

            // Ajouter le filtre département si spécifié (département 68 = Haut-Rhin)
            if (!string.IsNullOrEmpty(departement))
            {
                var departementLibelle = GetDepartementLibelle(departement);
                // Format: refine=lieu_declaration_facette:"Haut-Rhin"
                query += $"&refine=lieu_declaration_facette%3A%22{Uri.EscapeDataString(departementLibelle)}%22";
            }

            var url = $"{BaseUrl}?{query}";
            
            Console.WriteLine($"[JO API v2.1] URL requête: {url}");

            var response = await _httpClient.GetAsync(url);
            
            Console.WriteLine($"[JO API v2.1] Status: {response.StatusCode}");
            
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"[JO API v2.1] Réponse (premiers 1000 caractères): {content.Substring(0, Math.Min(1000, content.Length))}");

            var apiResponse = JsonSerializer.Deserialize<AssociationApiResponse>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            Console.WriteLine($"[JO API v2.1] Nombre de résultats: {apiResponse?.TotalResults ?? 0}");

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
            Console.WriteLine($"[JO API v2.1] Erreur: {ex.Message}");
            Console.WriteLine($"[JO API v2.1] StackTrace: {ex.StackTrace}");
            throw new Exception($"Erreur lors de la recherche d'associations : {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convertit un code département en libellé pour le filtre de l'API
    /// Mapping basé sur lieu_declaration_facette de l'API JO
    /// </summary>
    private string GetDepartementLibelle(string departementCode)
    {
        return departementCode switch
        {
            "68" => "Haut-Rhin",
            "67" => "Bas-Rhin",
            "75" => "Paris",
            "69" => "Rhône",
            "13" => "Bouches-du-Rhône",
            "33" => "Gironde",
            "44" => "Loire-Atlantique",
            "59" => "Nord",
            "06" => "Alpes-Maritimes",
            "31" => "Haute-Garonne",
            _ => departementCode // Si non trouvé, retourner le code tel quel
        };
    }
}
