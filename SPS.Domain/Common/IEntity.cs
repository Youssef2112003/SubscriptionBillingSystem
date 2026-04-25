namespace SPS.Domain.Common;

public interface IEntity<TId>
{
    TId Id { get; set; }
}