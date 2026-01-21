using CagnotteSolidaire.Domain.Contracts;
using CagnotteSolidaire.Domain.ValueObjects;

namespace CagnotteSolidaire.Domain.Entities;

public enum StatutCagnotte
{
    Ouverte,
    Cloturee,
    Annulee
}

public class Cagnotte(
    int id,
    string nom,
    string description,
    decimal objectif,
    int gestionnaireId,
    string? imageUrl) : Entity(id)
{
    public Label Nom { get; } = nom;
    public string Description { get; } = description;
    public Montant Objectif { get; } = objectif;
    public string? ImageUrl { get; set; } = imageUrl;
    public StatutCagnotte Statut { get; private set; } = StatutCagnotte.Ouverte;
    public int GestionnaireId { get; } = gestionnaireId;

    private List<Participation> _participations = new();
    public IReadOnlyList<Participation> Participations => _participations.AsReadOnly();

    public decimal MontantTotal => _participations.Sum(p => p.Montant.Value);

    public void AjouterParticipation(Participation participation)
    {
        if (Statut != StatutCagnotte.Ouverte)
            throw new ApplicationException("Cannot participate in closed cagnotte");

        _participations.Add(participation);
    }

    public void Cloturer()
    {
        if (MontantTotal < Objectif.Value)
            throw new ApplicationException("Objectif not reached");

        Statut = StatutCagnotte.Cloturee;
    }

    public void Annuler()
    {
        Statut = StatutCagnotte.Annulee;
    }

    public void SetImageUrl(string? url)
    {
        ImageUrl = url;
    }
}
