using RabbitMQ.Client;
using SimpleRabbitMq.Core.Interfaces;

namespace SimpleRabbitMq.Core;

public abstract class BaseConsumer<T> : IDisposable
	where T : class
{
    private readonly bool _temp = false;
    private readonly string _name = null;
    private readonly IModel _channel;
    private readonly List<ulong> _acks = new List<ulong>();
    private readonly List<ulong> _okAcks = new List<ulong>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="temp">Indictes if this consumer is temporary.
    /// It means that messages received when this consumer is inactive, will be lost</param>
    public BaseConsumer(IChannelsManager channelsManager, bool temp = false, string name = null)
    {
        _temp = temp;
        var eventType = typeof(T);
        var consumerType = this.GetType();

        _name = string.IsNullOrEmpty(name) ? $"{consumerType.FullName}+{eventType.FullName}" : name;

        _channel = channelsManager.CreateChannel<T>(null, typeof(T).FullName, false, _name, false);
    }

    /// <summary>
    /// Gets an item from the queue.
    /// </summary>
    /// <returns>The object retrieved from the queue</returns>
    public T Get()
    {
        var model = _channel.BasicGet(_name, false);

        if (model == null)
            return null;

        _acks.Add(model.DeliveryTag);

        return Serialization.DeserializeFromRabbitMQ<T>(model.Body.ToArray());
    }

    /// <summary>
    /// Sends the pending acks
    /// </summary>
    public void SendAcks()
    {
        foreach (var dt in _acks)
        {
            _channel.BasicAck(dt, false);
            _okAcks.Add(dt);
        }
    }

    /// <summary>
    /// Dispose the instance
    /// </summary>
    public void Dispose()
    {
        // Sending NoAck for pending acks
        foreach (var dt in _acks.Where(c => !_okAcks.Contains(c)))
        {
            _channel.BasicNack(dt, false, true);
        }

        _channel.Close(200, "Goodbye");
        _channel.Dispose();
    }
}
