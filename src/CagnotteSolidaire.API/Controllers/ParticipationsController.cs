using CagnotteSolidaire.Application.Participations.Commands.CreateParticipation;
using CagnotteSolidaire.Application.Participations.Queries.GetParticipantsByCagnotte;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CagnotteSolidaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ParticipationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParticipationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Participer à une cagnotte (Participant uniquement)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> CreateParticipation([FromBody] CreateParticipationCommand command)
    {
        // Vérifier que l'utilisateur participe à sa propre cagnotte
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (command.ParticipantId != userId)
        {
            return Forbid();
        }

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { participationId = result.Value, message = "Participation enregistrée avec succès" });
    }

    /// <summary>
    /// Obtenir la liste des participants d'une cagnotte
    /// </summary>
    [HttpGet("cagnotte/{cagnotteId}")]
    [Authorize(Roles = "Gestionnaire")]
    public async Task<IActionResult> GetParticipantsByCagnotte(Guid cagnotteId)
    {
        var query = new GetParticipantsByCagnotteQuery { CagnotteId = cagnotteId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
