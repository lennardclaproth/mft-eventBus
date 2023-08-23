namespace LClaproth.MyFinancialTracker.EventBus.RabbitMQ;

public class RabbitConfiguration
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Hostname { get; set; }
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; } = 5672;
}
