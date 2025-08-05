using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Common;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace RealEstate.Infrastructure.Repositorios
{
    /// <summary>
    /// Generic repository implementation for CRUD operations using Entity Framework Core.
    /// Provides common data access functionality for all entity types with support for:
    /// - Basic CRUD operations
    /// - Filtering, sorting, and paging
    /// - Eager loading of related entities
    /// - No-tracking queries
    /// </summary>
    /// <typeparam name="TEntity">The entity type this repository works with.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class 
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entity to the database asynchronously.
        /// Note: Changes are not saved until SaveChangesAsync() is called.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The added entity.</returns>
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var entry = await _context.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        /// <summary>
        /// Retrieves all entities of type TEntity from the database asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A list of all entities.</returns>
        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves an entity by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// Updates an existing entity in the database asynchronously.
        /// Note: This method marks the entity as modified but doesn't save changes to the database.
        /// Call SaveChangesAsync() to persist changes.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entry = _context.Update(entity);
            return Task.FromResult(entry.Entity);
        }

        /// <summary>
        /// Saves all changes made in this context to the database asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves the first entity that matches the specified filter, with optional includes, ordering and tracking behavior.
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="OrderBy">A function to sort the elements.</param>
        /// <param name="asNoTracking">Whether to disable change tracking for the query (default: false).</param>
        /// <param name="Includes">Navigation properties to include in the query.</param>
        /// <returns>The first matching entity, or null if no match is found.</returns>
        public async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy = null,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] Includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            // Include specified navigation properties
            foreach (Expression<Func<TEntity, object>> include in Includes)
            {
                query = query.Include(include);
            }

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply ordering if provided
            if (OrderBy != null)
            {
                query = OrderBy(query);
            }

            // Configure tracking behavior
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

             return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves all entities that match the specified filter, with optional includes, ordering and tracking behavior.
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="OrderBy">A function to sort the elements.</param>
        /// <param name="asNoTracking">Whether to disable change tracking for the query (default: false).</param>
        /// <param name="Includes">Navigation properties to include in the query.</param>
        /// <returns>A list of matching entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            int pageNumber, int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            // Include specified navigation properties
            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                query = query.Include(include);
            }

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply ordering if provided
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Configure tracking behavior
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
            return await query.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves all entities of type TEntity from the database asynchronously.
        /// </summary>
        /// <param name="asNoTracking">Whether to disable change tracking for the query (default: false).</param>
        /// <returns>A list of all entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
        {
            return await (asNoTracking ? _context.Set<TEntity>().AsNoTracking() : _context.Set<TEntity>()).ToListAsync();
        }

        /// <summary>
        /// Retrieves all entities that match the specified filter.
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="asNoTracking">Whether to disable change tracking for the query (default: false).</param>
        /// <returns>A list of matching entities.</returns>
        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false)
        {
            return await (asNoTracking ? _context.Set<TEntity>().AsNoTracking() : _context.Set<TEntity>()).Where(filter).ToListAsync();
        }

        /// <summary>
        /// Counts the number of entities that match the specified filter asynchronously.
        /// If no filter is provided, counts all entities of the type.
        /// </summary>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <returns>The number of entities that match the filter.</returns>
        // Get Count
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            

            query = query.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();
        }

        /// <summary>
        /// Retrieves a paged list of entities that match the specified filter and ordering.
        /// </summary>
        /// <param name="PageNumber">The 1-based page number to retrieve.</param>
        /// <param name="PageSize">The number of items per page.</param>
        /// <param name="filter">A function to test each element for a condition.</param>
        /// <param name="OrderBy">A function to sort the elements.</param>
        /// <param name="asNoTracking">Whether to disable change tracking for the query (default: false).</param>
        /// <param name="Includes">Navigation properties to include in the query.</param>
        /// <returns>A paged list of matching entities.</returns>
        public async Task<IEnumerable<TEntity>> GetPagedAsync(
            int PageNumber,
            int PageSize,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] Includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            // Include specified navigation properties
            foreach (Expression<Func<TEntity, object>> include in Includes)
            {
                query = query.Include(include);
            }

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply ordering if provided
            if (OrderBy != null)
            {
                query = OrderBy(query);
            }

            // Configure tracking behavior
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            // Apply paging
            int skip = (PageNumber - 1) * PageSize;
            query = query.Skip(skip).Take(PageSize);

            return await query.ToListAsync();
        }

        public void Delete(TEntity entity)
        {
            if (entity is BaseEntity softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                _context.Entry(entity).State = EntityState.Modified;
            } else
            {

                _context.Remove(entity);
            }
        }

        public DbContext GetDbContext() => _context;

         
    }
}