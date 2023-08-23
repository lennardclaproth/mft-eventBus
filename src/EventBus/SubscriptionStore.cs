namespace LClaproth.MyFinancialTracker.EventBus;

/**
    <summary>
        This class is responsible for managing and storing the subscriptions on an event.
    </summary>
**/
public class SubscriptionStore {

    private List<Subscription> _subscriptions = new List<Subscription>();

    public SubscriptionStore(){}
    
    public void AddSubscription(Type eventType, Type handlerType){
        var @event = (IntegrationEvent?) Activator.CreateInstance(eventType);
        string eventName = @event!.Name;
        _subscriptions.Add(new Subscription(eventName, eventType, handlerType));
    }

    public IDictionary<string, List<Subscription>> GetSubscriptions(){
        return _subscriptions
            .GroupBy(s => s.eventName)
            .ToDictionary(group => group.Key, group => group.ToList());
    }

    public IEnumerable<Type> GetEventHandlers(string eventName) {
        return _subscriptions
            .Where(s => eventName.Equals(s.eventName))
            .Select(s => s.HandlerType);
    }
}