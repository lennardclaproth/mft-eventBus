using Microsoft.Extensions.DependencyInjection;

namespace LClaproth.MyFinancialTracker.EventBus;

public static class SubscriptionLoaderExtensions
{
    public static IServiceCollection RegisterEventHandlers(this IServiceCollection services)
        {
            var handlerTypes = SubscriptionLoader.GetHandlerTypesFromAssembly();

            foreach (var handlerType in handlerTypes)
            {
                services.AddTransient(handlerType);
            }

            return services;
        }
}
