using CagnotteSolidaire.Application.Cagnottes.Commands.CancelCagnotte;
using CagnotteSolidaire.Application.Cagnottes.Commands.CloseCagnotte;
using CagnotteSolidaire.Application.Cagnottes.Commands.CreateCagnotte;
using CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnotteById;
using CagnotteSolidaire.Application.Cagnottes.Queries.GetCagnottesByGestionnaire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CagnotteSolidaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CagnottesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CagnottesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Créer une nouvelle cagnotte (Gestionnaire uniquement)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Gestionnaire")]
    public async Task<IActionResult> CreateCagnotte([FromBody] CreateCagnotteCommand command)
    {
        // Vérifier que le gestionnaire crée sa propre cagnotte
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (command.GestionnaireId != userId)
        {
            return Forbid();
        }

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetCagnotteById), 
            new { id = result.Value }, 
            new { id = result.Value });
    }

    /// <summary>
    /// Obtenir les détails d'une cagnotte
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCagnotteById(Guid id)
    {
        var query = new GetCagnotteByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { error = "Cagnotte introuvable" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtenir toutes les cagnottes d'un gestionnaire (Dashboard)
    /// </summary>
    [HttpGet("gestionnaire")]
    [Authorize(Roles = "Gestionnaire")]
    public async Task<IActionResult> GetMyCagnottes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var query = new GetCagnottesByGestionnaireQuery { GestionnaireId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Clôturer une cagnotte (objectif atteint)
    /// </summary>
    [HttpPost("{id}/close")]
    [Authorize(Roles = "Gestionnaire")]
    public async Task<IActionResult> CloseCagnotte(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new CloseCagnotteCommand
        {
            CagnotteId = id,
            GestionnaireId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Cagnotte clôturée avec succès" });
    }

    /// <summary>
    /// Annuler une cagnotte (objectif non atteint)
    /// </summary>
    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Gestionnaire")]
    public async Task<IActionResult> CancelCagnotte(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new CancelCagnotteCommand
        {
            CagnotteId = id,
            GestionnaireId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Cagnotte annulée avec succès" });
    }
}

