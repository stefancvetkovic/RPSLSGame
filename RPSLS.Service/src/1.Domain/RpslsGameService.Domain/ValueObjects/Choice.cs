using RpslsGameService.Domain.Enums;

namespace RpslsGameService.Domain.ValueObjects;

public sealed class Choice : IEquatable<Choice>
{
    public int Id { get; }
    public string Name { get; }
    public ChoiceType Type { get; }

    private Choice(int id, string name, ChoiceType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public static Choice Rock => new(1, "Rock", ChoiceType.Rock);
    public static Choice Paper => new(2, "Paper", ChoiceType.Paper);
    public static Choice Scissors => new(3, "Scissors", ChoiceType.Scissors);
    public static Choice Lizard => new(4, "Lizard", ChoiceType.Lizard);
    public static Choice Spock => new(5, "Spock", ChoiceType.Spock);

    public static Choice FromId(int id)
    {
        return id switch
        {
            1 => Rock,
            2 => Paper,
            3 => Scissors,
            4 => Lizard,
            5 => Spock,
            _ => throw new ArgumentException($"Invalid choice id: {id}", nameof(id))
        };
    }

    public static Choice FromType(ChoiceType type)
    {
        return type switch
        {
            ChoiceType.Rock => Rock,
            ChoiceType.Paper => Paper,
            ChoiceType.Scissors => Scissors,
            ChoiceType.Lizard => Lizard,
            ChoiceType.Spock => Spock,
            _ => throw new ArgumentException($"Invalid choice type: {type}", nameof(type))
        };
    }

    public static IReadOnlyList<Choice> GetAll() => new[] { Rock, Paper, Scissors, Lizard, Spock };

    public bool Equals(Choice? other)
    {
        if (other is null) return false;
        return Id == other.Id && Type == other.Type;
    }

    public override bool Equals(object? obj) => Equals(obj as Choice);

    public override int GetHashCode() => HashCode.Combine(Id, Type);

    public static bool operator ==(Choice? left, Choice? right) => Equals(left, right);

    public static bool operator !=(Choice? left, Choice? right) => !Equals(left, right);

    public override string ToString() => Name;
}