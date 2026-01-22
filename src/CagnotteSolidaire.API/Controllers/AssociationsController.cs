using CagnotteSolidaire.Application.Associations.Queries.SearchAssociations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CagnotteSolidaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssociationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssociationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Rechercher des associations via l'API Journal Officiel
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchAssociations(
        [FromQuery] string searchTerm,
        [FromQuery] string departement = "68")
    {
        var query = new SearchAssociationsQuery
        {
            SearchTerm = searchTerm,
            Departement = departement
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
