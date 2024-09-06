using RabbitMQ.Client;

namespace SimpleRabbitMq.Core.Interfaces;

public interface IChannelsManager
{
    void CloseConnections();

    IModel CreateChannel<T>(
        Action<T> onEvent = null,
        string exchangeName = null,
        bool temp = true,
        string queueName = null,
        bool listener = true,
        int? maxPriority = null,
        string type = "fanout",
        bool exchangeDurable = false
    );
}
