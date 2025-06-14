using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.ValueObjects;

namespace RpslsGameService.Domain.Services;

public class ChoiceGenerationService : IChoiceGenerationService
{
    public Choice GenerateComputerChoice(int randomNumber)
    {
        var normalizedNumber = ((randomNumber - 1) % 5) + 1;
        return Choice.FromId(normalizedNumber);
    }
}