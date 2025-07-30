using FluentResults;
using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.Auth;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Data;
using RealEstate.Infrastructure.Identity.jwt;

namespace RealEstate.Infrastructure.Repositorios.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            JwtTokenGenerator jwtTokenGenerator
            ) {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._jwtTokenGenerator = jwtTokenGenerator;
        }

        public  async Task<Result<string>> Login(LoginDto login)
        {
            var user = await _userManager.FindByNameAsync(login.username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, login.password))
            {
                return Result.Fail(new ConflictError("Credentials","Invalid Username or Password",enApiErrorCode.InvalidCredentials));
            } 


            var token = await _jwtTokenGenerator.GenerateToken(user);

            return Result.Ok(token);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
