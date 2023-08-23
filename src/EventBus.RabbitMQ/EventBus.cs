using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LClaproth.MyFinancialTracker.EventBus.RabbitMQ;

public class EventBus : IEventBusController, IEventBus, IAsyncDisposable
{
    private readonly ConnectionHandler _connections;
    private readonly EventBusHelper _helper;
    private readonly SubscriptionStore _subscriptions = new SubscriptionStore();

    private bool _isRunning = false;

    public EventBus(ConnectionHandler connections, EventBusHelper helper)
    {
        _helper = helper;
        _connections = connections;
    }

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
    public Task Publish<TEvent>(TEvent @event)
        where TEvent : IntegrationEvent, new()
    {
        // throw new NotImplementedException();
        var eventname = @event.Name;
        var exchangeName = eventname;

        DeclareExchange(eventname);

        var json = JsonSerializer.Serialize(@event);
        var buffer = Encoding.UTF8.GetBytes(json);

        lock (_connections.PublishChannel)
        {
            var properties = _connections.PublishChannel.CreateBasicProperties();
            properties.MessageId = @event.Id.ToString();
            properties.Type = @event.Name;

            _connections.PublishChannel.BasicPublish(exchange: exchangeName,
                routingKey: @event.Name,
                basicProperties: properties,
                body: buffer);
        }
        return Task.FromResult(0);
    }

    public Task Start()
    {
        _isRunning = true;
        foreach (var subscription in _subscriptions.GetSubscriptions())
        {
            var eventName = subscription.Key;

            BindToQueue(eventName);
        }

        return Task.FromResult(0);
    }

    private void OnMessageReceived(object? model, BasicDeliverEventArgs args)
    {
        var buffer = args.Body.ToArray();
        var eventname = args.BasicProperties.Type;

        var @event = _helper.DeserializeEventObject(eventname, buffer);
        var handlerTypes = _subscriptions.GetEventHandlers(eventname);

        Task.Run(async () =>
        {
            await _helper.InvokeEventHandlers(@event, handlerTypes);
        });
    }

    public Task Stop()
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
    public Task Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent, new()
        where THandler : IIntegrationEventHandler<TEvent>
    {
        _subscriptions.AddSubscription(typeof(TEvent), typeof(THandler));
        IntegrationEvent @event = (IntegrationEvent)Activator.CreateInstance(typeof(TEvent));

        DeclareExchange(@event.Name);
        DeclareQueue(@event.Name);

        BindToQueue(@event.Name);

        return Task.FromResult(0);
    }

    public bool DeclareQueue(string eventName)
    {
        _connections.SubscriptionChannel.QueueDeclare(
            queue: $"{eventName}:{Environment.GetEnvironmentVariable("ApplicationName")}",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        return true;
    }

    private bool DeclareExchange(string eventName)
    {
        _connections.SubscriptionChannel
            .ExchangeDeclare(exchange: eventName,
                type: ExchangeType.Direct,
                durable: true);

        return true;
    }

    private bool BindToQueue(string eventName)
        {
            var queueName = $"{eventName}:{Environment.GetEnvironmentVariable("ApplicationName")}";
            var exchangeName = eventName;

            var consumer = new EventingBasicConsumer(_connections.SubscriptionChannel);
            consumer.Received += OnMessageReceived;

            _connections.SubscriptionChannel
                .QueueBind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: exchangeName); // ? let op routingkey, oorspronkelijke waarde was: queueName

            _connections.SubscriptionChannel
                .BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);

            return true;
        }

    public Task UnSubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        throw new NotImplementedException();
    }
}