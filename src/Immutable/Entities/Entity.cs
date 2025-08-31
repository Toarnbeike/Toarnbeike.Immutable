namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Base record for an immutable entity with a unique strongly typed identifier.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract record Entity<TKey> : IEntity<TKey>
    where TKey : struct, IEntityKey<TKey>
{
    /// <inheritdoc />
    public TKey Id { get; }

    /// <summary>
    /// Constructor for a new entity with a generated ID.
    /// </summary>
    protected Entity()
    {
        Id = TKey.New();
    }

    /// <summary>
    /// Constructor for an existing entity with a specified ID.
    /// </summary>
    /// <param name="id">The id of the existing entity.</param>
    protected Entity(TKey id)
    {
        Id = id;
    }

    /// <inheritdoc />
    public Guid GetId() => Id.Value;
}