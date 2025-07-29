using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Features.Categories.Commands.NewFolder;
using RealEstate.Application.Features.Ratings.Commands.Create;
using RealEstate.Application.Features.Ratings.Commands.Update;
using RealEstate.Application.Features.Ratings.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {


        private readonly ISender _mediator;

        public RatingsController(ISender mediator)
        {
            _mediator = mediator;
        }
         
        [HttpGet] 
        public async Task<IActionResult> GetRentalsByPropertyId(
            [FromQuery] PaginationRequest pagination,
            [FromQuery] Guid? propertyId)
        {
            var Rentals = await _mediator.Send(new GetPropertyRatingsQuery(propertyId,pagination));
            return Rentals.Result.IsFailed ? Rentals.Result.ToActionResult(): Ok(Rentals.Data);
        }
        [HttpGet("is-rating")]
        [Authorize]
        public async Task<IActionResult> IsRating( [FromQuery] Guid? propertyId)
        {
            var Rentals = await _mediator.Send(new IsRatingByUserQuery(propertyId));
            return Rentals.Result.IsFailed ? Rentals.Result.ToActionResult() : Ok(Rentals.Data);
        }
        [HttpPost("property-Id/{propertyId}")]
        public async Task<IActionResult> CreateRating(
            [FromRoute] string? propertyId,
        [FromBody] CreateUpdateRatingDTO ratingData)
        {
            var command = new CreateRatingCommand(ratingData,propertyId);
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return response.Result.IsFailed ? response.Result.ToActionResult() : Created();
        }


        [HttpPut("Rating/Id/{ratingId}")] 
        public async Task<ActionResult<Guid>> UpdateRating(
        [FromRoute] string? ratingId,
        [FromBody] CreateUpdateRatingDTO ratingData)
        {
            var command = new UpdateRatingCommand(ratingData, ratingId);
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();
        }


    }
}
