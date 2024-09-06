using System.Reflection;
using SimpleRabbitMq.Core.Interfaces;

namespace SimpleRabbitMq.Core;

public class Consumer<T> : BaseConsumer<T>
    where T : class
{
    public Consumer(IChannelsManager channelsManager, bool temp = false, string name = null)
        : base(
            channelsManager,
            temp,
            $"{Assembly.GetExecutingAssembly().GetName().Name}.{typeof(T).FullName}"
        ) { }
}
