using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CagnotteSolidaire.Infrastructure.ExternalServices.Models;

public class AssociationApiResponse
{
    [JsonPropertyName("total_count")]
    public int TotalResults { get; set; }

    [JsonPropertyName("results")]
    public List<AssociationRecord> Records { get; set; } = new();
}

public class AssociationRecord
{
    // Champs directement au niveau du record (pas dans un sous-objet "fields")
    [JsonPropertyName("titre")]
    public string? Nom { get; set; }

    [JsonPropertyName("numero_rna")]
    public string? RNA { get; set; }

    [JsonPropertyName("dca_siren")]
    public string? SIREN { get; set; }

    [JsonPropertyName("commune_actuelle")]
    public string? Ville { get; set; }

    [JsonPropertyName("codepostal_actuel")]
    public string? CodePostal { get; set; }

    [JsonPropertyName("adresse_actuelle")]
    public string? AdresseComplete { get; set; }

    [JsonPropertyName("objet")]
    public string? Objet { get; set; }

    [JsonPropertyName("departement_code")]
    public string? DepartementCode { get; set; }

    [JsonPropertyName("departement_libelle")]
    public string? DepartementLibelle { get; set; }

    // Propriété calculée pour compatibilité avec le code existant
    public AssociationFields Fields => new AssociationFields
    {
        Nom = this.Nom,
        RNA = this.RNA,
        SIREN = this.SIREN,
        Ville = this.Ville,
        CodePostal = this.CodePostal,
        NumeroVoie = null, // N'existe plus dans la nouvelle API
        LibelleVoie = this.AdresseComplete // Adresse complète dans un seul champ
    };
}

public class AssociationFields
{
    public string? Nom { get; set; }
    public string? SIREN { get; set; }
    public string? RNA { get; set; }
    public string? Ville { get; set; }
    public string? CodePostal { get; set; }
    public string? NumeroVoie { get; set; }
    public string? LibelleVoie { get; set; }
}
