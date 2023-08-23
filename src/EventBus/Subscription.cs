namespace LClaproth.MyFinancialTracker.EventBus
{
    public record Subscription(string eventName, Type EventType, Type HandlerType) { }
}
