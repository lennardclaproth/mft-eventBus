using System.Text.Json.Serialization;

namespace LClaproth.MyFinancialTracker.EventBus;

public abstract class IntegrationEvent
{
    public abstract string Name { get; }
    public Guid Id { get; init; }
    public DateTime CreationDate { get; init; }

    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}