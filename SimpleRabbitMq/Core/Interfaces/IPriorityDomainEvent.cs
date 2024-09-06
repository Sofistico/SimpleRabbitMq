 namespace SimpleRabbitMq.Core.Interfaces;

 public interface IPriorityDomainEvent : IDomainEvent
 {
    public byte Priority {get; set;}
 }
