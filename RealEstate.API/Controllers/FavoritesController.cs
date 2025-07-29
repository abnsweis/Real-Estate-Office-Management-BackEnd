using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Features.Comments.Commands.Create;
using RealEstate.Application.Features.Favorites.Commands.Create;
using RealEstate.Application.Features.Favorites.Commands.Remove;
using RealEstate.Application.Features.Favorites.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ISender _mediator;

        public FavoritesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFavorite([FromQuery] PaginationRequest pagination)
        {
            var response = await _mediator.Send(new GetUserFavoritesQuery(pagination));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }
        [HttpGet("is-n-favorite/{propertyId}")]
        public async Task<IActionResult> IsInFavorite([FromRoute] Guid propertyId)
        {
            var response = await _mediator.Send(new IsInFavoriteQuery(propertyId));
            return Ok(new { IsInFavorite=response.Data});
        }



        [HttpPost("{propertyId}")]
        public async Task<IActionResult> AddToFavorites([FromRoute] Guid propertyId)
        {
            var command = new AddToFavoritesCommand(propertyId);
            var response = await _mediator.Send(command);

            return response.Result.IsFailed ? response.Result.ToActionResult(): Ok();  
        }


        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> RemoveFromFavorites([FromRoute] Guid propertyId)
        {
            var command = new RemoveFromFavoritesCommand(propertyId);
            var response = await _mediator.Send(command);

            return response.Result.IsFailed ? response.Result.ToActionResult() : NoContent();
        }
    }
}
