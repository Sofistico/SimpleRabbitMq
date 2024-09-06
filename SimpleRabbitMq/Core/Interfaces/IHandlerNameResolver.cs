namespace SimpleRabbitMq.Core.Interfaces;

public interface IHandlerNamesResolver<in T> : IHandler<T>
    where T : IDomainEvent
{
    void Handle(T args, INamesResolver namesResolver);
}
