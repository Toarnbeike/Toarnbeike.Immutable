namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Specifies that a record is an Aggregate
/// Source generates private constructors and static factory methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AggregateAttribute : Attribute;