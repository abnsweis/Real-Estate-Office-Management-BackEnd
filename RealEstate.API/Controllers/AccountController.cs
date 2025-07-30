using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.Auth;
using RealEstate.Application.Dtos.Users;
using RealEstate.Application.Features.Auth.Commands.Login;
using RealEstate.Application.Features.Users.Commands.Create;
using RealEstate.Application.Features.Users.Commands.UpdateCurrentUser;
using RealEstate.Application.Features.Users.Querys.GetUser;
using System.Linq.Dynamic.Core.Tokenizer;
namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    { 
        private readonly IAuthService _authService;
        private readonly ISender _mediator;

        public AccountController(IAuthService authService, ISender mediator)
        { 
            this._authService = authService;
            this._mediator = mediator;
        }

         
        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromForm]CreateUserDto createUserDTO)
        {
            var response = await _mediator.Send(new CreateUserCommand(createUserDTO));

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return Created() ;


        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var response = await _mediator.Send(new LoginCommand(model));

            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(new { Token = response.Data });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _mediator.Send(new LogoutCommand());

            if (!response.Result.IsSuccess)
                return response.Result.ToActionResult();

            return Ok();
        }
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetProfile()
        {
            var response = await _mediator.Send(new GetProfileQuery());

            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }
        [Authorize]
        [HttpPut("my/edite")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateCurrentUserDto dto)
        {
            var command = new UpdateCurrentUserCommand(dto);
            var response = await _mediator.Send(command);

            return response.Result.IsFailed
                ? response.Result.ToActionResult()
                : Ok(response.Data);
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken(TokenDto dto) {
        //    return NotFound();
        //}


        //[Authorize]
        //[HttpPost("change-password")]
        //public async Task<IActionResult> ChangePassword(Chan 
        //    return NotFound();
        //}

        //[HttpPost("forgot-password")]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        //{
        //    return NotFound();
        //}

        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        //{
        //    return NotFound();
        //}

        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(string userId, string token)
        //{
        //    return NotFound();
        //}

        //[Authorize]
        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    return NotFound();
        //}

    }
} 