namespace Toarnbeike.Immutable.Abstractions.Entities;

/// <summary>
/// Specifies that a record is an Aggregate
/// Source generates private constructors and static factory methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AggregateAttribute : EntityAttribute;

[AttributeUsage(AttributeTargets.Class)]
public class EntityAttribute : Attribute
{
    /// <summary>
    /// The name of the aggregate when used pluralized, e.g. as collection name in the dataContext.
    /// </summary>
    public string? PluralName { get; set; }
    
    /// <summary>
    /// Version number of the aggregate. Allows for migration type logic.
    /// Defaults to 1 for the initial version.
    /// Incrementing this version requires a SnapshotMigration to keep compatibility with older versions.
    /// It is not needed to increment the version while no older snapshots are in active use.
    /// </summary>
    public int Version { get; set; } = 1; 
}