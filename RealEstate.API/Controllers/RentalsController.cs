using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Rental;
using RealEstate.Application.Features.Rentals.Commands;
using RealEstate.Application.Features.Rentals.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly ISender _mediator;
        public RentalsController(ISender mediator)
        {
            _mediator = mediator;
        }



        /// <summary>
        /// Retrieves all Rentals with pagination.
        /// </summary>
        /// <param name="pagination">Pagination parameters (PageNumber, PageSize)</param> 
        /// <returns>A paginated list of Rentals.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<RentalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllRentals([FromQuery] PaginationRequest pagination)
        {
            var response = await _mediator.Send(new GetAllRentalsQuery(pagination));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        /// <summary>
        /// Retrieves a single Rental by its unique ID.
        /// </summary>
        /// <param name="RentalId">The ID of the Rental</param>
        /// <returns>The matched Rental if found</returns>
        [HttpGet("Id/{RentalId:guid}")]
        [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRentalById(Guid RentalId)
        {
            var response = await _mediator.Send(new GetRentalByIdQuery(RentalId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpGet("Property/{PropertyId:guid}")]
        public async Task<IActionResult> GetRentalsByPropertyId([FromQuery] PaginationRequest pagination, Guid PropertyId)
        {
            var response = await _mediator.Send(new GetRentalsByPropertyIdQuery(pagination, PropertyId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpGet("Lessee/{LesseeId:guid}")]
        public async Task<IActionResult> GetRentalsBySellerId([FromQuery] PaginationRequest pagination, Guid LesseeId)
        {
            var response = await _mediator.Send(new GetRentalsByLesseeIdQuery(pagination, LesseeId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpGet("Lessor/{LessorId:guid}")]
        public async Task<IActionResult> GetRentalsByBuyerId([FromQuery] PaginationRequest pagination, Guid LessorId)
        {
            var response = await _mediator.Send(new GetRentalsByLessorIdQuery(pagination, LessorId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }


        /// <summary>
        /// Creates a new Rental.
        /// </summary>
        /// <param name="RentalData">The Rental data to create</param>
        /// <returns>The created Rental's ID</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRental([FromForm] CreateRentalDTO RentalData)
        {
            var Command = new CreateRentalCommand(RentalData);
            var response = await _mediator.Send(Command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return CreatedAtAction(
                nameof(GetRentalById),
                new { RentalId = response.Data },
                new { RentalId = response.Data });
        }

    }
}
