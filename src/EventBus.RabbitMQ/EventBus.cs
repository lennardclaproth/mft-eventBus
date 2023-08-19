namespace LClaproth.MyFinancialTracker.EventBus.RabbitMQ;

class EventBus : IEventBus, IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    /**
        <summary> 
            This method will publish an event to the exchange. The name of the exchange is the <br />
            name of the event. The routing key, the key of the queue(s) which are connected <br />
            is the same as the name of the event.
        </summary>
    */
    public void Publish<TEvent>(TEvent @event)
    {
        throw new NotImplementedException();
    }

    /**
        <summary> 
            This method is responsible for subscribing to an exchange. It creates a queue based on <br />
            the name of the event and the name of the application. The exchange to which the queue <br />
            will be connected is also the name of the event. The queue will receive messages based <br />
            on its routing key which will be the name of the event.
        </summary>
    */
    public void Subscribe<TEvent, THandler>(TEvent @event, THandler handler)
    {
        throw new NotImplementedException();
    }

    
    public void UnSubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}