namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Strongly typed entity key interface.
/// </summary>
public interface IEntityKey<out TSelf>
    where TSelf : struct, IEntityKey<TSelf>
{
    /// <summary>
    /// Internal value of the entity key.
    /// Preferable use <code>Guid.CreateVersion7()</code> for the construction
    /// of lexicographically sortable keys.
    /// </summary>
    Guid Value { get; }

    /// <summary>
    /// Static factory method to create a new instance of the IEntityKey.
    /// </summary>
    static abstract TSelf New();
}