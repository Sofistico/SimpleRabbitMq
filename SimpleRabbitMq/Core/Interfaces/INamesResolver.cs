namespace SimpleRabbitMq.Core.Interfaces;

public interface INamesResolver
{
    /// <summary>
    /// Builds the queue name
    /// </summary>
    /// <typeparam name="T">The type of the event that will be handled</typeparam>
    /// <param name="onEvent">Event that will handle the messages from the queue</param>
    /// <returns>String with the queue name</returns>
    string QueueName<T>(Action<T> onEvent);

    /// <summary>
    /// Builds Exchange name
    /// </summary>
    /// <param name="type">The type of the event that will be queued</param>
    /// <returns>String with the excahnge name. The default is the FullName of the type </returns>
    string ExchangeName(Type type);
}
