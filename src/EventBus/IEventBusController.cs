namespace LClaproth.MyFinancialTracker.EventBus;

public interface IEventBusController : IEventBus
{
    Task Start();

    Task Stop();
}
