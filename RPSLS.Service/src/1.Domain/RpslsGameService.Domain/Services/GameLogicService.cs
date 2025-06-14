using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.ValueObjects;

namespace RpslsGameService.Domain.Services;

public class GameLogicService : IGameLogicService
{
    private static readonly IReadOnlyDictionary<ChoiceType, IReadOnlyList<ChoiceType>> WinConditions = 
        new Dictionary<ChoiceType, IReadOnlyList<ChoiceType>>
        {
            { ChoiceType.Rock, new[] { ChoiceType.Lizard, ChoiceType.Scissors } },
            { ChoiceType.Paper, new[] { ChoiceType.Rock, ChoiceType.Spock } },
            { ChoiceType.Scissors, new[] { ChoiceType.Paper, ChoiceType.Lizard } },
            { ChoiceType.Lizard, new[] { ChoiceType.Spock, ChoiceType.Paper } },
            { ChoiceType.Spock, new[] { ChoiceType.Scissors, ChoiceType.Rock } }
        };

    public GameResult DetermineWinner(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice == null) throw new ArgumentNullException(nameof(playerChoice));
        if (computerChoice == null) throw new ArgumentNullException(nameof(computerChoice));

        var outcome = CalculateOutcome(playerChoice, computerChoice);
        return new GameResult(playerChoice, computerChoice, outcome);
    }

    public IReadOnlyList<Choice> GetAllChoices()
    {
        return Choice.GetAll();
    }

    private static GameOutcome CalculateOutcome(Choice playerChoice, Choice computerChoice)
    {
        if (playerChoice.Type == computerChoice.Type)
        {
            return GameOutcome.Tie;
        }

        return WinConditions[playerChoice.Type].Contains(computerChoice.Type)
            ? GameOutcome.Win
            : GameOutcome.Lose;
    }
}