using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleRabbitMq.Core.Interfaces;

namespace SimpleRabbitMq.Core;

public class ChannelManager : IChannelsManager, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly INamesResolver _namesResolver;

    private static IConnection _connection;
    private static List<IModel> _channels;

    private IConnection Connection => _connection ?? (_connection = CreateConnection());
    private static List<IModel> Channels => _channels ?? (_channels = new List<IModel>());

    public ChannelManager(IConfiguration configuration)
    {
        _configuration = configuration;
        _namesResolver = new DefaultNameResolver();
    }

    public ChannelManager(IConfiguration configuration, INamesResolver namesResolver)
    {
        _configuration = configuration;
        _namesResolver = namesResolver;
    }

    private IConnection CreateConnection()
    {
        var rabbitMqSettings = _configuration.GetSection("RabbitMq");
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqSettings["Host"],
            UserName = rabbitMqSettings["User"],
            Password = rabbitMqSettings["Password"],
            VirtualHost = rabbitMqSettings["VirtualHost"],
            AutomaticRecoveryEnabled = Convert.ToBoolean(
                rabbitMqSettings["AutomaticRecoveryEnabled"]
            ),
            TopologyRecoveryEnabled = Convert.ToBoolean(
                rabbitMqSettings["TopologyRecoveryEnabled"]
            ),
            RequestedHeartbeat = TimeSpan.FromSeconds(
                Convert.ToUInt16(rabbitMqSettings["RequestedHeartbeat"])
            ),
            NetworkRecoveryInterval = TimeSpan.FromSeconds(
                Convert.ToUInt16(rabbitMqSettings["NetworkRecoveryInterval"])
            ),
            ContinuationTimeout = TimeSpan.FromSeconds(
                Convert.ToUInt16(rabbitMqSettings["ContinuationTimeout"])
            )
        };

        return factory.CreateConnection();
    }

    public void CloseConnections()
    {
        if(_channels != null){
            foreach (var channel in _channels)
            {
                channel.Close(200, "Goodbye");
                channel.Dispose();
            }
            _channels.Clear();
            _channels.Capacity = 0;
            _channels = null;
        }
        if (_connection?.IsOpen == true)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="onEvent"></param>
    /// <param name="exchangeName"></param>
    /// <param name="temp"></param>
    /// <param name="queueName"></param>
    /// <param name="listener">Listener or consumer</param>
    /// <returns></returns>
    public IModel CreateChannel<T>(
        Action<T> onEvent = null,
        string exchangeName = null,
        bool temp = true,
        string queueName = null,
        bool listener = true,
        int? maxPriority = null,
        string type = "fanout",
        bool exchangeDurable = false
    )
    {
        queueName = !string.IsNullOrEmpty(queueName)
            ? queueName
            : _namesResolver.QueueName(onEvent);

        exchangeName = exchangeName ?? _namesResolver.ExchangeName(typeof(T));

        var channel = Connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchangeName, type: type, durable: exchangeDurable);

        if (temp)
        {
            queueName = channel.QueueDeclare().QueueName;
        }
        else if (maxPriority.HasValue)
        {
            var arguments = new Dictionary<string, object>
            {
                { "x-max-priority", (byte)maxPriority.Value }
            };
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
            );
        }
        else
        {
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        channel.BasicQos(0, 1, false);

        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        if (onEvent != null)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                try
                {
                    onEvent(Serialization.DeserializeFromRabbitMQ<T>(body.ToArray()));

                    // Sending ACK to the queue
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch
                {
                    // Sending NACK to requeue the message
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        if (listener)
        {
            Channels.Add(channel);
        }

        return channel;
    }

    public void Dispose()
    {
        CloseConnections();
    }
}
