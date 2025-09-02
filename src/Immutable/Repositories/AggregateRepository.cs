using Toarnbeike.Immutable.Entities;
using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Optional;
using Toarnbeike.Optional.Collections;
using Toarnbeike.Results;
using Toarnbeike.Results.Extensions;

namespace Toarnbeike.Immutable.Repositories;

//todo: consider creating specific failures for EntityNotFound
/// <summary>
/// Abstract base class for aggregate repository implementations.
/// Provides default implementations for common CRUD operations while delegating
/// storage to a <see cref="Dictionary{TKey, TValue}"/> that is injected
/// by the source-generated concrete implementation.
/// </summary>
/// <typeparam name="TEntity">The aggregate root entity type that implements <see cref="IEntity{TKey}"/></typeparam>
/// <typeparam name="TKey">The strongly-typed entity key that implements <see cref="IEntityKey{TKey}"/></typeparam>
/// <param name="mutationStore">The mutation store used for change tracking and undo/redo functionality</param>
public abstract class AggregateRepository<TEntity, TKey>(IMutationStore mutationStore)  : IAggregateRepository<TEntity, TKey>
    where TEntity : IEntity<TKey>
    where TKey : struct, IEntityKey<TKey> 
{
    private readonly string _entityName = typeof(TEntity).Name;
    
    /// <summary>
    /// The underlying storage collection for entities. This field is populated by the 
    /// source-generated concrete implementation with a reference to the appropriate 
    /// collection from the DataContext.
    /// </summary>
    protected Dictionary<TKey, TEntity> Entities = null!;
    
    /// <summary>
    /// The mutation store used for tracking changes and enabling undo/redo functionality.
    /// </summary>
    protected readonly IMutationStore MutationStore = mutationStore;
    
    /// <inheritdoc />
    public Result<TEntity> GetById(TKey id) =>
        Entities.TryGetValue(id, out var entity)
            ? entity
            : new Failure(_entityName, $"Entity with id {id} not found.");

    /// <inheritdoc />
    public Option<TEntity> FirstOrNone(Func<TEntity, bool> predicate) =>
        Entities.Values.FirstOrNone(predicate);

    /// <inheritdoc />
    public IEnumerable<TEntity> GetAll() =>
        Entities.Values;

    /// <inheritdoc />
    public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate) =>
        Entities.Values.Where(predicate);

    /// <inheritdoc />
    public Result Add(TEntity entity, string? changeDescription = null)
    {
        return AddImpl(entity)
            .Tap(() => MutationStore.RegisterMutation(new Mutation(
                () => RemoveImpl(entity.Id),
                () => AddImpl(entity),
                changeDescription)));
    }

    /// <summary>
    /// The actual action of adding the entity to the collection.
    /// Verifies that there is no entity with this Id already present.
    /// </summary>
    /// <remarks>
    /// Used to separate the actual adding of the entity from the mutation handling.
    /// </remarks>
    private Result AddImpl(TEntity entity) =>
        Result.Success(entity)
            .Check(ent => !Entities.ContainsKey(ent.Id), () =>
                new Failure(_entityName, $"Entity with id {entity.Id} already exists."))
            .Tap(ent => Entities.Add(ent.Id, entity));

    /// <inheritdoc />
    public Result Update(TEntity updatedEntity, string? changeDescription = null)
    {
        return UpdateImpl(updatedEntity)
            .Tap(originalEntity => MutationStore.RegisterMutation(new Mutation(
                () => UpdateImpl(originalEntity),
                () => UpdateImpl(updatedEntity),
                changeDescription)));
    }

    /// <summary>
    /// The actual action of updating the entity to the collection.
    /// Verifies that there is already an entity with this Id.
    /// </summary>
    /// <remarks>
    /// Used to separate the actual updating of the entity from the mutation handling.
    /// </remarks>
    /// <returns>A result containing the original state of the entity,
    /// to make undo possible.</returns>
    private Result<TEntity> UpdateImpl(TEntity updatedEntity)
    {
        // todo: What if entity is unchanged? Do nothing or return new Failure()?
        return Entities.TryGetValue(updatedEntity.Id, out var originalEntity)
            ? Result.Success(originalEntity).Tap(_ => Entities[updatedEntity.Id] = updatedEntity)
            : new Failure(_entityName, $"Entity with id {updatedEntity.Id} not found.");
    }

    /// <inheritdoc />
    public Result Remove(TKey id, string? changeDescription = null)
    {
        return RemoveImpl(id)
            .Tap(originalEntity => MutationStore.RegisterMutation(new Mutation(
                () => AddImpl(originalEntity),
                () => RemoveImpl(id),
                changeDescription)));
    }

    /// <summary>
    /// The actual action of updating the entity to the collection.
    /// Verifies that there is already an entity with this Id.
    /// </summary>
    /// <remarks>
    /// Used to separate the actual updating of the entity from the mutation handling.
    /// </remarks>
    /// <returns>A result containing the original state of the entity,
    /// to make undo possible.</returns>
    private Result<TEntity> RemoveImpl(TKey id)
    {
        return  Entities.TryGetValue(id, out var originalEntity)
            ? Result.Success(originalEntity).Tap(_ => Entities.Remove(id))
            : new Failure(_entityName, $"Entity with id {id} not found.");
    }
}