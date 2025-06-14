namespace RpslsGameService.Application.DTOs;

public record ChoiceDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}