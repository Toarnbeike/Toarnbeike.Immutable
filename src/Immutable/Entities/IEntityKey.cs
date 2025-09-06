using Toarnbeike.Immutable.Abstractions.Entities;

namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Strongly typed entity key interface.
/// </summary>
public interface IEntityKey<out TSelf> : IEntityKey
    where TSelf : struct, IEntityKey<TSelf>
{
    /// <summary>
    /// Creates a new instance of the <typeparam name="TSelf" /> with a new Version 7 GUID.
    /// Uses Guid.CreateVersion7() for lexicographically sortable keys.
    /// </summary>
    /// <returns>A new <typeparam name="TSelf" /> instance.</returns>
    static abstract TSelf New();

    /// <summary>
    /// Creates a new empty <typeparam name="TSelf" />.
    /// </summary>
    /// <returns>An empty <typeparam name="TSelf" /> instance.</returns>
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    static TSelf Empty { get; }
}