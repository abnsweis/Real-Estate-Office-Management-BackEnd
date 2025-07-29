using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Users;
using RealEstate.Application.Features.Users.Commands.Create;
using RealEstate.Application.Features.Users.Commands.Delete;
using RealEstate.Application.Features.Users.Commands.Update;
using RealEstate.Application.Features.Users.Commands.Update.UpdateUserImage;
using RealEstate.Application.Features.Users.Querys.Check;
using RealEstate.Application.Features.Users.Querys.GetUser;
using RealEstate.Application.Features.Users.Querys.ReadAll;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISender _mediator;

        public UsersController(ISender mediator) {
            this._mediator = mediator;
        }





        [HttpPut("{userId}")]
        public async Task<ActionResult<Guid>> Update([FromRoute] Guid userId, [FromForm] UpdateUserDto userDto, CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand(userDto);
            command.Data.UserId = userId;
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();


        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationRequest pagination ,[FromQuery] FiltterUserDTO filtter)
        {
            var users = await _mediator.Send(new GetAllUsersQuery(pagination, filtter));
            return Ok(users);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserById(Guid Id)
        {
            var results = await _mediator.Send(new GetUserByIdQuery(Id));
            return results.ToActionResult();
        }
        [HttpGet("Count")]
        public async Task<IActionResult> GetUsersCount()
        {
            var count = await _mediator.Send(new GetUserCountQuery());
            return Ok(new
            {
                TotalCountUsers = count
            });
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid Id)
        {
            var response = await _mediator.Send(new DeleteUserCommand(Id));

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();
        }

        [HttpPut("{Id}/UpdateImage")]
        public async Task<IActionResult> UpdateImage([FromRoute]Guid Id, [FromForm] UpdateUserImageDto userImageDto)
        {
            var response = await _mediator.Send(new UpdateUserImageCommand(Id, userImageDto.Image));

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();
        }

        [HttpGet("exists-username")]
        public async Task<IActionResult> ExistsUsername([FromQuery] string username)
        {
            var exists = await _mediator.Send(new ExistsUserQuery(username,enCheckTo.Username));
            return Ok(exists);
        }

        [HttpGet("exists-email")]
        public async Task<IActionResult> ExistsEmail([FromQuery] string email)
        {
            var exists = await _mediator.Send(new ExistsUserQuery(email,enCheckTo.Email));
            return Ok(exists);
        }
        [HttpGet("exists-phoneNumber")]
        public async Task<IActionResult> ExistsPhoneNumber([FromQuery] string PhoneNumber)
        {
            var exists = await _mediator.Send(new ExistsUserQuery(PhoneNumber, enCheckTo.Phone));
            return Ok(exists);
        }
    }
}
