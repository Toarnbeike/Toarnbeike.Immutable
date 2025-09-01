namespace Toarnbeike.Immutable.Entities;

/// <summary>
/// Specifies that a record is an EntityKey
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class EntityKeyAttribute : Attribute;