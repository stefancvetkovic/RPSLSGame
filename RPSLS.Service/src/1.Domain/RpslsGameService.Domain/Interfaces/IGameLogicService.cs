using RpslsGameService.Domain.ValueObjects;

namespace RpslsGameService.Domain.Interfaces;

public interface IGameLogicService
{
    GameResult DetermineWinner(Choice playerChoice, Choice computerChoice);
    IReadOnlyList<Choice> GetAllChoices();
}