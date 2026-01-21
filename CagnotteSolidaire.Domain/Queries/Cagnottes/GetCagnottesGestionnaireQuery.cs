using MediatR;

namespace CagnotteSolidaire.Domain.Queries.Cagnottes;

public class GetCagnottesGestionnaireQuery(int gestionnaireId) : IRequest<List<CagnottesDTO>>
{
    public int GestionnaireId { get; } = gestionnaireId;
}
