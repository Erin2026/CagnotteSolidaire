using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json.Serialization;

namespace CagnotteSolidaire.Infrastructure.ExternalServices.Models;

public class AssociationApiResponse
{
    [JsonPropertyName("nhits")]
    public int TotalResults { get; set; }

    [JsonPropertyName("records")]
    public List<AssociationRecord> Records { get; set; } = new();
}

public class AssociationRecord
{
    [JsonPropertyName("fields")]
    public AssociationFields Fields { get; set; } = new();
}

public class AssociationFields
{
    [JsonPropertyName("titre")]
    public string? Nom { get; set; }

    [JsonPropertyName("id_siren")]
    public string? SIREN { get; set; }

    [JsonPropertyName("id_rna")]
    public string? RNA { get; set; }

    [JsonPropertyName("adresse_libelle_commune")]
    public string? Ville { get; set; }

    [JsonPropertyName("adresse_code_postal")]
    public string? CodePostal { get; set; }

    [JsonPropertyName("adresse_numero_voie")]
    public string? NumeroVoie { get; set; }

    [JsonPropertyName("adresse_libelle_voie")]
    public string? LibelleVoie { get; set; }
}