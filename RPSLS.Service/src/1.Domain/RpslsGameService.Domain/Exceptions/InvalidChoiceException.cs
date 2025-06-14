namespace RpslsGameService.Domain.Exceptions;

public class InvalidChoiceException : DomainException
{
    public int? InvalidId { get; }

    public InvalidChoiceException(int id) 
        : base($"Invalid choice ID: {id}. Valid IDs are 1-5.")
    {
        InvalidId = id;
    }

    public InvalidChoiceException(string message) 
        : base(message)
    {
    }
}