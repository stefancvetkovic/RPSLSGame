using FluentValidation;
using RpslsGameService.Application.DTOs;

namespace RpslsGameService.Application.Validators;

public class PlayGameRequestValidator : AbstractValidator<PlayGameRequest>
{
    public PlayGameRequestValidator()
    {
        RuleFor(x => x.Player)
            .InclusiveBetween(1, 5)
            .WithMessage("Player choice must be between 1 and 5 (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock)");
    }
}