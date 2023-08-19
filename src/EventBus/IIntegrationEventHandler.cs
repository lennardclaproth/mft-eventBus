namespace LClaproth.MyFinancialTracker.EventBus;

public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}