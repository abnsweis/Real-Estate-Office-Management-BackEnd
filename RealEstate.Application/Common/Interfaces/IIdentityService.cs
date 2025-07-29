
using FluentResults;
using RealEstate.Application.Dtos.Users;
using RealEstate.Application.Features.Users.Commands.Create;

namespace RealEstate.Application.Common.Interfaces
{
    public interface IIdentityService
    {

        Task<Result<Guid>> CreateUserAsync(CreateUserCommand registerCommand, CancellationToken cancellationToken);

        Task<List<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<Result<string?>> GetUserNameAsync(Guid userId);
        Task<UserDTO?> GetUserIdAsync(Guid userId);

        Task<bool> IsInRoleAsync(Guid userId, string role);

        Task<bool> AuthorizeAsync(Guid userId, string policyName);


        Task<Result> DeleteUserAsync(Guid userId);
    }
}
