using SimpleRabbitMq.Core.Interfaces;

namespace SimpleRabbitMq.Core;

public class Listener : IListener
{
    private readonly IChannelsManager _channelsManager;

    public Listener(IChannelsManager channelsManager)
    {
        _channelsManager = channelsManager;
    }

    public void AddHandler<T>(
        Action<T> onEvent,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    )
    {
        if (onEvent == null)
            throw new ArgumentException("onEvent cannot be null");
        for (int i = 0; i < listenerAmount; i++)
        {
            _channelsManager.CreateChannel<T>(
                onEvent,
                temp: false,
                queueName: queueName,
                maxPriority: maxPriority,
                type: type,
                exchangeDurable: exchangeDurable
            );
        }
    }

    public void AddHandler<T>(
        Action<T> onEvent,
        string exchangeName,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    )
    {
        if (onEvent == null)
            throw new ArgumentException("onEvent cannot be null");

        for (int i = 0; i < listenerAmount; i++)
        {
            _channelsManager.CreateChannel<T>(
                onEvent,
                exchangeName,
                false,
                queueName: queueName,
                maxPriority: maxPriority,
                type: type,
                exchangeDurable: exchangeDurable
            );
        }
    }

    public void AddHandler<T>(
        IHandler<T> handler,
        string exchangeName,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    ) where T : IDomainEvent 
    {
        if (handler == null)
            throw new ArgumentException("handler cannot be null");
        //Quando ser uma implementação de um IDomainEvent a fila não deve ser temporária
        for (int i = 0; i < listenerAmount; i++)
        {
            _channelsManager.CreateChannel<T>(
                handler.Handle,
                exchangeName,
                false,
                queueName: queueName,
                maxPriority: maxPriority,
                type: type,
                exchangeDurable: exchangeDurable
            );
        }
    }

    public void AddHandler<T>(
        IHandler<T> handler,
        int listenerAmount = 1,
        int? maxPriority = null,
        string queueName = null,
        string type = "fanout",
        bool exchangeDurable = false
    )
        where T : IDomainEvent 
    {
        if (handler == null)
            throw new ArgumentException("handler cannot be null");
        //Quando ser uma implementação de um IDomainEvent a fila não deve ser temporária
        for (int i = 0; i < listenerAmount; i++)
        {
            _channelsManager.CreateChannel<T>(
                handler.Handle,
                temp: false,
                queueName: queueName,
                maxPriority: maxPriority,
                type: type,
                exchangeDurable: exchangeDurable
            );
        }
    }

    public void AddTempHandler<T>(Action<T> onEvent, int listenerAmount = 1)
    {
        if (onEvent == null)
            throw new ArgumentException("onEvent cannot be null");
        for (int i = 0; i < listenerAmount; i++)
        {
            _channelsManager.CreateChannel<T>(onEvent, temp: true);
        }
    }

    public void AddTempHandler<T>(Action<T> onEvent, string exchangeName, int listenerAmount = 1)
    {
        if (onEvent == null)
            throw new ArgumentException("onEvent cannot be null");
        for (int i = 0; i < listenerAmount; i++)
        {
            _channelsManager.CreateChannel<T>(onEvent, exchangeName, true);
        }
    }

    public void CloseConnections()
    {
        _channelsManager.CloseConnections();
    }
}
