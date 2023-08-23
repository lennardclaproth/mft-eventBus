using Microsoft.Extensions.Hosting;

namespace LClaproth.MyFinancialTracker.EventBus;

public class EventBusService : IHostedService
{

    public readonly IEventBusController _bus;

    public EventBusService(IEventBusController bus)
    {
        _bus = bus;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var loader = new SubscriptionLoader(_bus);
        loader.LoadHandlersFromAssembly();
        
        return Task.FromResult(_bus.Start());
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
