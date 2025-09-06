namespace Toarnbeike.Immutable.Abstractions.Entities;

/// <summary>
/// Contract for a domain entity.
/// When possible, prefer the use of <see cref="IEntity{TKey}"/> for a strongly typed Id.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Helper method to get the unique identifier of the entity.
    /// This is used for generic access, when the IEntityKey{TKey} type is unknown.
    /// </summary>
    Guid GetId();
}