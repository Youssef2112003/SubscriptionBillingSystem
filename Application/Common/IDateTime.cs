namespace SPS.Application.Common;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}