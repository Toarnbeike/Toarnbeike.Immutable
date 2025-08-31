namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Contract for a domain entity that acts as an aggregate root in the domain.
/// </summary>
public interface IAggregate<out TKey> : IEntity<TKey>
    where TKey : struct, IEntityKey<TKey>;