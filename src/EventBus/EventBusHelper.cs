using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

// TODO: Should properly check how this works.

namespace LClaproth.MyFinancialTracker.EventBus
{
    public class EventBusHelper
    {
        private readonly IServiceScopeFactory _factory;

        public EventBusHelper(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task InvokeEventHandlers<TEvent>(TEvent @event, IEnumerable<Type> eventHandlerTypes)
            where TEvent : IntegrationEvent
        {
            using (var scope = _factory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var tasks = new List<Task>();

                foreach (var handlerType in eventHandlerTypes)
                {
                    var handler = provider.GetService(handlerType);
                    var task = handlerType.GetMethod("Handle")
                        !.Invoke(handler, new object[] { @event }) as Task;

                    tasks.Add(task!);
                }

                await Task.WhenAll(tasks);
            }
        }

        public IntegrationEvent DeserializeEventObject(string eventname, ReadOnlySpan<byte> buffer)
        {
            var eventType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract &&
                    type.IsClass &&
                    type.IsAssignableTo(typeof(IntegrationEvent)))
                .Select(type => Activator.CreateInstance(type))
                .Cast<IntegrationEvent>()
                .First(instance => instance.Name == eventname)
                .GetType();

            var @event = (IntegrationEvent?)JsonSerializer.Deserialize(buffer, eventType);
            return @event!;
        }
    }
}