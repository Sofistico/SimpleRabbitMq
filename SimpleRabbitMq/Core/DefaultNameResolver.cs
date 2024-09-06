using SimpleRabbitMq.Core.Interfaces;

namespace SimpleRabbitMq.Core;

public class DefaultNameResolver : INamesResolver
{
    /// <summary>
    /// Builds the queue name.
    /// </summary>
    /// <typeparam name="T">The type of the event that will be handled</typeparam>
    /// <param name="onEvent">Event that will handle the messages from the queue</param>
    /// <returns>String with the queue name</returns>
    public virtual string QueueName<T>(Action<T> onEvent)
    {
        return onEvent == null
            ? typeof(T).FullName
            : $"{onEvent.Method.DeclaringType.FullName}+{typeof(T).FullName}";
    }

    /// <summary>
    /// Builds Exchange name
    /// </summary>
    /// <param name="type">The type of the event that will be queued</param>
    /// <typeparam name="T">The type of the event that will be queued</typeparam>
    /// <returns>String with the excahnge name. The default is the FullName of the type </returns>
    public virtual string ExchangeName(Type type)
    {
        return type.FullName;
    }
}
