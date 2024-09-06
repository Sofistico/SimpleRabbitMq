using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleRabbitMq.Core.Configuration;
using SimpleRabbitMq.Core.Interfaces;

namespace SimpleRabbitMq.Core;

public class DomainEventDefaultHandler : IDisposable
{
    private readonly IConnection _connection;
    private readonly INamesResolver _namesResolver;

    protected DomainEventDefaultHandler(RabbitMqConnection settings)
        : this(settings, new DefaultNameResolver()) { }

    protected DomainEventDefaultHandler(RabbitMqConnection settings, INamesResolver namesResolver)
    {
        _connection = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost,
            AutomaticRecoveryEnabled = settings.AutomaticRecoveryEnabled,
            TopologyRecoveryEnabled = settings.TopologyRecoveryEnabled,
            RequestedHeartbeat = settings.RequestedHeartbeat,
            NetworkRecoveryInterval = settings.NetworkRecoveryInterval,
            ContinuationTimeout = settings.ContinuationTimeout
        }.CreateConnection();
        _namesResolver = namesResolver;
    }

    public void Dispose()
    {
        if (_connection.IsOpen)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }

    protected void HandleEvent(
        IDomainEvent args,
        int? delayMilliseconds = null,
        INamesResolver namesResolver = null
    )
    {
        namesResolver = namesResolver ?? _namesResolver ?? new DefaultNameResolver();
        var exch = namesResolver.ExchangeName(args.GetType());

        // Serializing object to send to Rabbit
        var body = Serialization.SerializeToRabbitMQ(args);

        using (var channel = _connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: exch, type: "fanout");

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            if (args is IPriorityDomainEvent @event)
                properties.Priority = @event.Priority;

            if (delayMilliseconds.HasValue)
            {
                var delayedQueue = $"{exch}.delayed.{delayMilliseconds}ms";
                channel.QueueDeclare(
                    delayedQueue,
                    true,
                    false,
                    false,
                    new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", exch },
                        { "x-dead-letter-routing-key", "" },
                        { "x-message-ttl", delayMilliseconds }
                    }
                );
                channel.BasicPublish(
                    exchange: "",
                    routingKey: delayedQueue,
                    basicProperties: properties,
                    body: body
                );
            }
            else
            {
                channel.BasicPublish(
                    exchange: exch,
                    routingKey: "",
                    basicProperties: properties,
                    body: body
                );
            }
        }
    }

    protected TResult HandleRepliedEvent<TRequest, TResult>(TRequest args)
    {
        var body = Serialization.SerializeToRabbitMQ(args);

        TResult returnObj = default;

        using (var channel = _connection.CreateModel())
        {
            var replyQueueName = channel
                .QueueDeclare(
                    queue: $"replied_event_callback_queue_{Guid.NewGuid()}",
                    durable: false,
                    exclusive: false,
                    autoDelete: true
                )
                .QueueName;

            var props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            //publish message to domain queue
            channel.BasicPublish(
                exchange: "",
                routingKey: $"replied_queue_{args.GetType().ToString().ToLower()}",
                basicProperties: props,
                body: body
            );

            //consume callback queue
            var consumer = new EventingBasicConsumer(channel);

            bool received = false;

            consumer.Received += (model, ea) =>
            {
                returnObj = Serialization.DeserializeFromRabbitMQ<TResult>(ea.Body.ToArray());
                received = true;
            };

            channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);

            while (!received)
            {
                Thread.Sleep(1);
            }
        }

        return returnObj;
    }
}

public class DomainEventHandler : DomainEventDefaultHandler, IHandlerNamesResolver<IDomainEvent>
{
    public DomainEventHandler(RabbitMqConnection settings)
        : base(settings) { }

    public DomainEventHandler(RabbitMqConnection settings, INamesResolver namesResolver)
        : base(settings, namesResolver) { }

    /// <summary>
    ///     Handles Cross Domain Events. All events will be queued in a RabbitMQ queue.
    /// </summary>
    /// <param name="args">Event to be handled</param>
    public void Handle(IDomainEvent args, INamesResolver namesResolver)
    {
        HandleEvent(args, namesResolver: namesResolver);
    }

    public void Handle(IDomainEvent args)
    {
        HandleEvent(args);
    }
}
