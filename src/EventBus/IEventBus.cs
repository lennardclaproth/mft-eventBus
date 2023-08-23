namespace LClaproth.MyFinancialTracker.EventBus;

public interface IEventBus
{
    Task Publish<TEvent>(TEvent @event) 
        where TEvent : IntegrationEvent, new();
        
    Task Subscribe<TEvent, THandler>() 
        where TEvent : IntegrationEvent, new()
        where THandler : IIntegrationEventHandler<TEvent>;

    Task UnSubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}
