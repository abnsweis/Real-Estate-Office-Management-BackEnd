using Azure;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination; 
using RealEstate.Application.Features.Categories.Commands;
using RealEstate.Application.Features.Categories.Commands.NewFolder;
using RealEstate.Application.Features.Testimonials.Commands.Create;
using RealEstate.Application.Features.Testimonials.Commands.Update;
using RealEstate.Application.Features.Testimonials.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialsController : ControllerBase
    {

        private readonly ISender _mediator;

        public TestimonialsController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all Testimonials with pagination
        /// </summary>
        /// <param name="pagination">Pagination parameters (page number and page size)</param>
        /// <returns>Paginated list of Testimonials</returns>
        /// <response code="200">Returns the Testimonials list</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllTestimonials([FromQuery] PaginationRequest pagination)
        {
            var response = await _mediator.Send(new GetAllTestimonialsQuery(pagination));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTestimonial([FromForm] string? RatingText, [FromForm] int? RatingNumber)
        {
            var command = new CreateTestimonialCommand(RatingText, RatingNumber);
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return Created();
        }

        [HttpPut("{testimonialId}")]
        public async Task<ActionResult<Guid>> UpdateTestimonial([FromRoute] Guid testimonialId, [FromForm] string? RatingText, [FromForm] int? RatingNumber)
                {
            var command = new UpdateTestimonialCommand(testimonialId,RatingText, RatingNumber);
            var response = await _mediator.Send(command);
            return response.Result.IsFailed ? response.Result.ToActionResult(): NoContent();
        }



    }
}
