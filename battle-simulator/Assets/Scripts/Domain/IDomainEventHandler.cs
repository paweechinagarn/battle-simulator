namespace BattleSimulator
{
    /// <summary>
    /// A handler for events that are raised via the DomainEvents class.
    /// </summary>
    public interface IDomainEventHandler
    {
    }

    /// <summary>
    /// A handler for events that are raised via the DomainEvents class. To build a class whose sole purpose is to take action
    /// when domain events are raised, implement this interface.
    /// </summary>
    public interface IDomainEventHandler<in T> : IDomainEventHandler where T : IDomainEvent
    {
        void Handle(T evt);
    }
}