using FluentResults;
using RealEstate.Application.Dtos.Auth;

namespace RealEstate.Application.Common.Interfaces
{
    public interface IAuthService
    {

        Task<Result<string>> Login(LoginDto login);   
        Task LogoutAsync();
    }
}
