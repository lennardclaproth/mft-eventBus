namespace LClaproth.MyFinancialTracker.EventBus;

public interface IEventBus
{
    void Publish<TEvent>(TEvent @event);
    void Subscribe<TEvent, THandler>(TEvent @event, THandler handler);
    void UnSubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}
