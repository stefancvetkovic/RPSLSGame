namespace RpslsGameService.Application.DTOs;

public record PlayGameRequest
{
    public int Player { get; init; }
}