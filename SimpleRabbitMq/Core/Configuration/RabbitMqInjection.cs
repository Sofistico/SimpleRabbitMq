using Microsoft.Extensions.DependencyInjection;
using SimpleRabbitMq.Core.Interfaces;
using SimpleRabbitMq.Core;

namespace SimpleRabbitMq.Core.Configuration
{
    public static class RabbitMqInjection
    {
        public static IServiceCollection AddRabbitMq(
            this IServiceCollection services,
            RabbitMqConnection settings
        )
        {
            services.AddSingleton(settings);
            services.AddSingleton<IHandler<IDomainEvent>, DomainEventHandler>();
            services.AddSingleton<IHandlerNamesResolver<IDomainEvent>, DomainEventHandler>();
            services.AddSingleton<IChannelsManager, ChannelManager>();
            services.AddTransient<IListener, Listener>();

            return services;
        }
    }
}
