namespace Toarnbeike.Immutable.Abstractions.Entities;

/// <summary>
/// Strongly typed entity key interface.
/// </summary>
public interface IEntityKey 
{
    /// <summary>
    /// Internal value of the entity key.
    /// Preferable use <code>Guid.CreateVersion7()</code> for the construction
    /// of lexicographically sortable keys.
    /// </summary>
    Guid Value { get; }
}