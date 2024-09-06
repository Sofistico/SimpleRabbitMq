namespace SimpleRabbitMq.Core.Interfaces;

/// <summary>
/// This interface must be implemented to raise and deal with the event
/// </summary>
/// <typeparam name="T">type to be dealt</typeparam>
public interface IHandler<in T>
    where T : IDomainEvent
{
    void Handle(T args);
}
