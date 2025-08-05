using FluentResults;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IUserRepository
    {
        // Count Users matching filter
        Task<int> CountAsync(Expression<Func<UserDomain, bool>>? filter = null);
        Task<int> AdminsCountAsync();

        // Add new user
        Task<AppResponse<Guid>> AddAsync(UserDomain user, CancellationToken cancellationToken = default);

        // Update existing user
        Task<AppResponse> UpdateAsync(UserDomain user);

        // Get user by ID
        Task<UserDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        // Get filtered, sorted list with optional tracking and includes
        Task<IEnumerable<UserDomain>> GetAllAsync(
            int pageNumber, int pageSize,
            Expression<Func<UserDomain, bool>>? filter = null,
            Func<IQueryable<UserDomain>, IOrderedQueryable<UserDomain>>? orderBy = null,
            bool asNoTracking = false,
            params Expression<Func<UserDomain, object>>[] includes);

        // Get all entities
        Task<IEnumerable<UserDomain>> GetAllAsync(bool asNoTracking = false);

        // Get filtered entities
        Task<IEnumerable<UserDomain>> GetWhereAsync(Expression<Func<UserDomain, bool>> filter, bool asNoTracking = false);

        // Save changes to database
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


        // Get first matching user
        Task<UserDomain?> FirstOrDefaultAsync(
            Expression<Func<UserDomain, bool>>? filter = null,
            Func<IQueryable<UserDomain>, IOrderedQueryable<UserDomain>>? orderBy = null,
            bool asNoTracking = false,
            params Expression<Func<UserDomain, object>>[] Includes);



        Task<AppResponse> Delete(Guid id);

        bool IsEmailAlreadyTaken(string email);
        bool IsUsernameAlreadyTaken(string username);
        bool IsPhoneNumberAlreadyTaken(string phoneNumber);
        bool IsUserExists(Guid id);
    }
}

