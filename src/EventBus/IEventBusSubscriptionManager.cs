namespace LClaproth.MyFinancialTracker.EventBus;

public interface IEventBusSubscriptionsManager
{
    bool IsEmpty();
    event EventHandler<string> OnEventRemoved;
    void addSubsciption<TEvent, THandler>() 
        where TEvent : IntegrationEvent 
        where THandler : IIntegrationEventHandler<TEvent>;

    void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

    Type GetEventTypeByName(string eventName);

    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}
