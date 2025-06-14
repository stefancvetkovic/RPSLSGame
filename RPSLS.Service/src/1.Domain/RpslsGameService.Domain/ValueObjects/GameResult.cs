using RpslsGameService.Domain.Enums;

namespace RpslsGameService.Domain.ValueObjects;

public sealed class GameResult
{
    public Choice PlayerChoice { get; }
    public Choice ComputerChoice { get; }
    public GameOutcome Outcome { get; }
    public string ResultMessage { get; }
    public DateTime PlayedAt { get; }

    public GameResult(Choice playerChoice, Choice computerChoice, GameOutcome outcome)
    {
        PlayerChoice = playerChoice ?? throw new ArgumentNullException(nameof(playerChoice));
        ComputerChoice = computerChoice ?? throw new ArgumentNullException(nameof(computerChoice));
        Outcome = outcome;
        ResultMessage = GenerateResultMessage(playerChoice, computerChoice, outcome);
        PlayedAt = DateTime.UtcNow;
    }

    private static string GenerateResultMessage(Choice playerChoice, Choice computerChoice, GameOutcome outcome)
    {
        if (outcome == GameOutcome.Tie)
        {
            return $"Both players chose {playerChoice.Name}. It's a tie!";
        }

        var verb = GetVerbForChoices(playerChoice, computerChoice);
        
        return outcome == GameOutcome.Win
            ? $"{playerChoice.Name} {verb} {computerChoice.Name}. You win!"
            : $"{computerChoice.Name} {verb} {playerChoice.Name}. You lose!";
    }

    private static string GetVerbForChoices(Choice choice1, Choice choice2)
    {
        return (choice1.Type, choice2.Type) switch
        {
            (ChoiceType.Rock, ChoiceType.Scissors) => "crushes",
            (ChoiceType.Rock, ChoiceType.Lizard) => "crushes",
            (ChoiceType.Paper, ChoiceType.Rock) => "covers",
            (ChoiceType.Paper, ChoiceType.Spock) => "disproves",
            (ChoiceType.Scissors, ChoiceType.Paper) => "cuts",
            (ChoiceType.Scissors, ChoiceType.Lizard) => "decapitates",
            (ChoiceType.Lizard, ChoiceType.Spock) => "poisons",
            (ChoiceType.Lizard, ChoiceType.Paper) => "eats",
            (ChoiceType.Spock, ChoiceType.Scissors) => "smashes",
            (ChoiceType.Spock, ChoiceType.Rock) => "vaporizes",
            (ChoiceType.Scissors, ChoiceType.Rock) => "crushes",
            (ChoiceType.Lizard, ChoiceType.Rock) => "crushes",
            (ChoiceType.Rock, ChoiceType.Paper) => "covers",
            (ChoiceType.Spock, ChoiceType.Paper) => "disproves",
            (ChoiceType.Paper, ChoiceType.Scissors) => "cuts",
            (ChoiceType.Lizard, ChoiceType.Scissors) => "decapitates",
            (ChoiceType.Spock, ChoiceType.Lizard) => "poisons",
            (ChoiceType.Paper, ChoiceType.Lizard) => "eats",
            (ChoiceType.Scissors, ChoiceType.Spock) => "smashes",
            (ChoiceType.Rock, ChoiceType.Spock) => "vaporizes",
            _ => "defeats"
        };
    }
}