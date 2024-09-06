namespace SimpleRabbitMq.Core.Interfaces;

public interface IListener
{
    void AddHandler<T>(
        Action<T> onEvent,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    );

    void AddHandler<T>(
        Action<T> onEvent,
        string exchangeName,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    );

    void AddHandler<T>(
        IHandler<T> handler,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    )
        where T : IDomainEvent;

    void AddHandler<T>(
        IHandler<T> handler,
        string exchangeName,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    )
        where T : IDomainEvent;

    void AddTempHandler<T>(Action<T> handler, int listenerAmount = 1);

    void AddTempHandler<T>(Action<T> handler, string exchangeName, int listenerAmount = 1);

    void CloseConnections();
}
