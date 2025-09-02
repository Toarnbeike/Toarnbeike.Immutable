using Toarnbeike.Immutable.Entities;
using Toarnbeike.Optional;
using Toarnbeike.Results;

namespace Toarnbeike.Immutable.Repositories;

/* todo: Additional methods might be useful here:
 * - GetAll with orderBySelector and ListSortDirection
 * - GetAllPaged, again with orderBySelector and ListSortDirection
 * - GetSingle: single entity or NotFoundFailure
 * - GetSingleOrNone: single entity or None
 * - GetFirst: first entity or NotFoundFailure
 * - GetFirstBy: first entity with OrderBySelector and ListSortDirection or NotFoundError
 * - GetFirstOrNoneBy: first entity with OrderBySelector and ListSortDirection or None
 * - AddMany: overload to add multiple TEntity entries at once
 * - Delete(TEntity): shorthand for Delete(entity.Id);
*/
/// <summary>
/// Base interface for repositories that manage aggregate root entities.
/// Provides both query operations and command operations with built-in change tracking.
/// </summary>
/// <typeparam name="TEntity">The aggregate root entity type that implements <see cref="IEntity{TKey}"/></typeparam>
/// <typeparam name="TKey">The strongly-typed entity key that implements <see cref="IEntityKey{TKey}"/></typeparam>
public interface IAggregateRepository<TEntity, in TKey>
    where TEntity : IEntity<TKey>
    where TKey : struct, IEntityKey<TKey>
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve</param>
    /// <returns>A <see cref="Result"/> containing the entity if found, or a failure if not found</returns>
    Result<TEntity> GetById(TKey id);
    
    /// <summary>
    /// Finds the first entity that matches the specified predicate, or returns none if no match is found.
    /// </summary>
    /// <param name="predicate">The condition to test each entity against</param>
    /// <returns>An <see cref="Option"/> containing the first matching entity, or <see cref="Option.None"/> if no match is found</returns>
    Option<TEntity> FirstOrNone(Func<TEntity, bool> predicate);
    
    /// <summary>
    /// Retrieves all entities in the repository.
    /// </summary>
    /// <returns>An enumerable collection of all entities</returns>
    /// <remarks>Use with caution on large datasets. Consider using <see cref="Where(Func{TEntity, bool})"/> for filtering.</remarks>
    IEnumerable<TEntity> GetAll();
    
    /// <summary>
    /// Filters entities based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to test each entity against</param>
    /// <returns>An enumerable collection of entities that satisfy the predicate</returns>
    IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="changeDescription">Optional description of the change for undo/redo tracking</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the operation</returns>
    /// <remarks>Fails if an entity with the same ID already exists</remarks>
    Result Add(TEntity entity, string? changeDescription = null);
    
    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="updatedEntity">The updated entity</param>
    /// <param name="changeDescription">Optional description of the change for undo/redo tracking</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the operation</returns>
    /// <remarks>Fails if the entity does not exist</remarks>
    Result Update(TEntity updatedEntity, string? changeDescription = null);
    
    /// <summary>
    /// Removes an entity from the repository by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to remove</param>
    /// <param name="changeDescription">Optional description of the change for undo/redo tracking</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the operation</returns>
    /// <remarks>Fails if the entity does not exist</remarks>
    Result Remove(TKey id, string? changeDescription = null);
}