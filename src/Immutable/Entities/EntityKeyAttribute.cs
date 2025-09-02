namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Specifies that a record is an EntityKey
/// Source generates static New() method and static Empty property.
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class EntityKeyAttribute : Attribute;