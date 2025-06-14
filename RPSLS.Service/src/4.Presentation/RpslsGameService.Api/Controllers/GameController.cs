using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Application.UseCases.Commands;
using RpslsGameService.Application.UseCases.Queries;

namespace RpslsGameService.Api.Controllers;

/// <summary>
/// Game controller for Rock, Paper, Scissors, Lizard, Spock game operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GameController> _logger;

    public GameController(IMediator mediator, ILogger<GameController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all available game choices
    /// </summary>
    /// <returns>List of available choices</returns>
    /// <response code="200">Returns the list of choices</response>
    [HttpGet("choices")]
    [ProducesResponseType(typeof(IReadOnlyList<ChoiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ChoiceDto>>> GetChoices(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting all game choices");
        
        var query = new GetChoicesQuery();
        var choices = await _mediator.Send(query, cancellationToken);
        
        return Ok(choices);
    }

    /// <summary>
    /// Get a random computer choice
    /// </summary>
    /// <returns>A random choice for the computer</returns>
    /// <response code="200">Returns a random choice</response>
    [HttpGet("choice")]
    [ProducesResponseType(typeof(ChoiceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChoiceDto>> GetRandomChoice(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting random computer choice");
        
        var query = new GetRandomChoiceQuery();
        var choice = await _mediator.Send(query, cancellationToken);
        
        return Ok(choice);
    }

    /// <summary>
    /// Play a game round
    /// </summary>
    /// <param name="request">Player's choice (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock)</param>
    /// <returns>Game result</returns>
    /// <response code="200">Returns the game result</response>
    /// <response code="400">Invalid player choice</response>
    [HttpPost("play")]
    [ProducesResponseType(typeof(GameResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameResultResponse>> PlayGame(
        [FromBody] PlayGameRequest request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Playing game round with player choice: {PlayerChoice}", request.Player);
        
        var command = new PlayGameCommand(request.Player);
        var result = await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation("Game round completed. Result: {Result}", result.Results);
        
        return Ok(result);
    }

    /// <summary>
    /// Get game history and statistics
    /// </summary>
    /// <returns>Game history with statistics</returns>
    /// <response code="200">Returns game history and stats</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(GameHistoryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GameHistoryResponse>> GetGameHistory(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting game history");
        
        var query = new GetGameHistoryQuery();
        var history = await _mediator.Send(query, cancellationToken);
        
        return Ok(history);
    }

    /// <summary>
    /// Reset game statistics and history
    /// </summary>
    /// <returns>Success confirmation</returns>
    /// <response code="204">Game statistics reset successfully</response>
    [HttpDelete("reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetGame(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting game statistics");
        
        var command = new ResetScoreboardCommand();
        await _mediator.Send(command, cancellationToken);
        
        _logger.LogInformation("Game statistics reset successfully");
        
        return NoContent();
    }
}