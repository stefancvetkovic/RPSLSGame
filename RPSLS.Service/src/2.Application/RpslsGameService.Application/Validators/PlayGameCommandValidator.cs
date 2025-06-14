using FluentValidation;
using RpslsGameService.Application.UseCases.Commands;

namespace RpslsGameService.Application.Validators;

public class PlayGameCommandValidator : AbstractValidator<PlayGameCommand>
{
    public PlayGameCommandValidator()
    {
        RuleFor(x => x.PlayerChoice)
            .InclusiveBetween(1, 5)
            .WithMessage("Player choice must be between 1 and 5 (1=Rock, 2=Paper, 3=Scissors, 4=Lizard, 5=Spock)");
    }
}