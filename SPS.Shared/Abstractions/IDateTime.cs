namespace SPS.Shared.Abstractions;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

