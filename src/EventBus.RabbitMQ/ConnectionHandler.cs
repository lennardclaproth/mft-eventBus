using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client.Exceptions;


namespace LClaproth.MyFinancialTracker.EventBus.RabbitMQ;

public class ConnectionHandler : IDisposable
{

    private readonly RabbitConfiguration _configuration;
    private readonly ILogger<ConnectionHandler> _logger;
    private IConnectionFactory _factory;

    private IConnection _publishConnection;
    private IModel _publishChannel;

    private object _publishChannelLock = new object(); // Used for locking

    public ConnectionHandler(RabbitConfiguration configuration, ILogger<ConnectionHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

    public IModel PublishChannel
    {
        get
        {
            if (_publishChannel == null)
            {
                lock (_publishChannelLock)
                {
                    if (_publishChannel == null)
                    {
                        _publishConnection ??= CreateConnection();
                        _publishChannel = _publishConnection.CreateModel();
                    }
                }
            }

            return _publishChannel;
        }
    }

    private IConnection _subscriptionConnection;
    private IModel _subscriptionChannel;

    private object _subscriptionChannelLock = new object(); // Used for locking
    public IModel SubscriptionChannel
    {
        get
        {
            if (_subscriptionChannel == null)
            {
                lock (_subscriptionChannelLock)
                {
                    if (_subscriptionChannel == null)
                    {
                        _subscriptionConnection ??= CreateConnection();
                        _subscriptionChannel = _subscriptionConnection.CreateModel();
                    }
                }
            }

            return _subscriptionChannel;
        }
    }

    private IConnection CreateConnection()
    {

        var policy = Policy.Handle<BrokerUnreachableException>().WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(5)
            },
            onRetry: (timeSpan, retryAttempt, context) =>
            {
                _logger.LogWarning($"Could not connect to the EventBroker, retrying in {timeSpan}. Attempt number: {retryAttempt}");
            }
        );
        // _factory = new ConnectionFactory();
        _factory = new ConnectionFactory()
        {
            HostName = _configuration.Hostname,
            UserName = _configuration.Username,
            Password = _configuration.Password,
            VirtualHost = _configuration.VirtualHost,
            Port = _configuration.Port
        };

        return policy.Execute(() => _factory.CreateConnection());
    }

    public void Dispose()
    {
        CloseAndDispose(_publishChannel);
        CloseAndDispose(_publishConnection);
        CloseAndDispose(_subscriptionChannel);
        CloseAndDispose(_subscriptionConnection);

        _publishChannel = null;
        _publishConnection = null;
        _subscriptionChannel = null;
        _subscriptionConnection = null;
        _factory = null;
    }

    private void CloseAndDispose(IModel? channel)
    {
        if (channel == null)
        {
            return;
        }

        if (channel.IsOpen)
        {
            channel.Close();
        }

        channel.Dispose();
    }

    private void CloseAndDispose(IConnection? connection)
    {
        if (connection == null)
        {
            return;
        }

        if (connection.IsOpen)
        {
            connection.Close();
        }

        connection.Dispose();
    }
}
