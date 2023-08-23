using LClaproth.MyFinancialTracker.EventBus;

internal static class SubscriptionHelper {
    public static Subscription CreateSubscription(Type eventType, Type handlerType){
        var @event = (IntegrationEvent?) Activator.CreateInstance(eventType);
        var eventName = @event!.Name;

        return new Subscription(eventName, eventType, handlerType);
    }

    public static Type getEventTypeFromHandler(Type handlerType){
        return handlerType.GetInterfaces()
            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
            .Select(@interface => @interface.GetGenericArguments().First())
            .First();
    }
}
