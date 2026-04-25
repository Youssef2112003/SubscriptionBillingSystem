namespace SPS.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Data { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; set; }

    public OutboxMessage(string type, string data, DateTime occurredOn)
    {
        Id = Guid.NewGuid();
        Type = type;
        Data = data;
        OccurredOn = occurredOn;
    }
}