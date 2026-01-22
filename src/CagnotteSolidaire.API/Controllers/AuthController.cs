using CagnotteSolidaire.Application.Auth.Commands.Login;
using CagnotteSolidaire.Application.Auth.Commands.RegisterGestionnaire;
using CagnotteSolidaire.Application.Auth.Commands.RegisterParticipant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CagnotteSolidaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Inscription d'un participant
    /// </summary>
    [HttpPost("register/participant")]
    public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { userId = result.Value, message = "Inscription réussie" });
    }

    /// <summary>
    /// Inscription d'un gestionnaire (avec association)
    /// </summary>
    [HttpPost("register/gestionnaire")]
    public async Task<IActionResult> RegisterGestionnaire([FromBody] RegisterGestionnaireCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { userId = result.Value, message = "Inscription réussie" });
    }

    /// <summary>
    /// Connexion (retourne un token JWT)
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return Unauthorized(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}
