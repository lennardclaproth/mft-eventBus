namespace LClaproth.MyFinancialTracker.EventBus;

/**
    <summary>
        This class is responsible for loading all the subscriptions based on the EventHandlers<br/>
    </summary>
**/
public class SubscriptionLoader
{
    private readonly IEventBus _eventBus;

    public SubscriptionLoader(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void LoadHandlersFromAssembly()
    {
        var subscriptions = BuildSubscriptions(GetHandlerTypesFromAssembly());

        var subsribeMethod = _eventBus.GetType().GetMethod("Subscribe");
        foreach (var subscription in subscriptions)
        {
            subsribeMethod
                !.MakeGenericMethod(subscription.EventType, subscription.HandlerType)
                .Invoke(_eventBus, null);
        }
    }

    /**
        <summary>
            Builds subscriptions based on the eventType of the eventHandler.
        </summary>
        <param name="handlerTypes" >Types of handlers to build subscriptions from.</param>
        <returns>A List of subscriptions</returns>
    **/
    private IEnumerable<Subscription> BuildSubscriptions(IEnumerable<Type> handlerTypes)
    {
        return handlerTypes.Select(t =>
            SubscriptionHelper.CreateSubscription(
                SubscriptionHelper.getEventTypeFromHandler(t),
                t));
    }

    public static IEnumerable<Type> GetHandlerTypesFromAssembly()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract &&
                type.IsClass &&
                type.GetInterfaces().Any(@interface => 
                    @interface.IsGenericType &&
                    @interface.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)));
    }
}