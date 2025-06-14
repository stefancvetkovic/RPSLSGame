using RpslsGameService.Domain.ValueObjects;

namespace RpslsGameService.Domain.Interfaces;

public interface IChoiceGenerationService
{
    Choice GenerateComputerChoice(int randomNumber);
}