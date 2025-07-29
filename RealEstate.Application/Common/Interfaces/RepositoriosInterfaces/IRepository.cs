using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    // Basic repository interface for CRUD operations
    public interface IRepository<TEntity> where TEntity : class
    {
        // Count entities matching filter
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);

        // Add new entity
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        // Update existing entity
        Task<TEntity> UpdateAsync(TEntity entity);
        void Delete(TEntity entity); 

        // Get entity by ID
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Get filtered, sorted list with optional tracking and includes
        Task<IEnumerable<TEntity>> GetAllAsync(
            int pageNumber, int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[]? includes);

        // Get all entities
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);

        // Get filtered entities
        Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false);

        // Save changes to database
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Get paged results
        Task<IEnumerable<TEntity>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);

        // Get first matching entity
        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);


        DbContext GetDbContext();
    }
}