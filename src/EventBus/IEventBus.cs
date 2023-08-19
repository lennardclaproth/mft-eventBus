public interface IEventBus
{
    void Publish<TEvent>(TEvent @event);
    void Subscribe<TEvent, THandler>(TEvent @event, THandler handler);
    void UnSubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}

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

public class SubscriptionInfo {
        public Type HandlerType { get; }

        private SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }

        public static SubscriptionInfo Typed(Type handlerType) =>
            new SubscriptionInfo(handlerType);
}

public class IntegrationEvent { }

public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}