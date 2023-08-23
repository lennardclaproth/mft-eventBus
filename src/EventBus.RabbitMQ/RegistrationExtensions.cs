using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LClaproth.MyFinancialTracker.EventBus.RabbitMQ;

public static class RegistrationExtensions
{
    public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var appConfiguration = provider.GetService<IConfiguration>();

        var rabbitConfiguration = new RabbitConfiguration();
        appConfiguration.Bind(nameof(RabbitConfiguration), rabbitConfiguration);

        return AddRabbitMQEventBus(services, rabbitConfiguration);
    }

    public static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, RabbitConfiguration configuration)
    {
        services
            .AddSingleton<RabbitConfiguration>(provider => configuration)
            .AddSingleton<ConnectionHandler>()
            .AddSingleton<EventBusHelper>()
            .AddSingleton<EventBus>()
            .AddSingleton<IEventBus, EventBus>(x => x.GetRequiredService<EventBus>())
            .AddSingleton<IEventBusController, EventBus>(x => x.GetRequiredService<EventBus>())
            .AddSingleton<SubscriptionStore>()
            .AddHostedService<EventBusService>();

        services.RegisterEventHandlers();

        return services;
    }
}
