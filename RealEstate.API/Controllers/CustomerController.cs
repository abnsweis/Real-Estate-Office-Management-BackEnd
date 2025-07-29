using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Features.Customers.Commands.Create;
using RealEstate.Application.Features.Customers.Commands.Delete;
using RealEstate.Application.Features.Customers.Commands.Update;
using RealEstate.Application.Features.Customers.Querys;
using RealEstate.Domain.Enums;

namespace RealEstate.API.Controllers
{
    /// <summary>
    /// Controller for managing customer operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {


        private readonly ISender _mediator;

        public CustomersController(ISender mediator)
        {
            this._mediator = mediator;
        }
        /// <summary>
        /// Get all customers with filtering and pagination capabilities
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="filtter">Customer filtering parameters</param>
        /// <returns>List of customers matching the criteria</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] PaginationRequest pagination, [FromQuery] FiltterCustomersDTO filtter)
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery(pagination, filtter));
            return Ok(customers);
        }


        /// <summary>
        /// Get all buyers
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>List of buyers</returns>
        [HttpGet("Buyers")]
        public async Task<IActionResult> GetBuyers([FromQuery] PaginationRequest pagination)
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery(pagination, new FiltterCustomersDTO { customerType = enCustomerType.Buyer.ToString()}));
            return Ok(customers);
        }


        /// <summary>
        /// Get all renters
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>List of renters</returns>
        [HttpGet("Renters")]
        public async Task<IActionResult> GetRenters([FromQuery] PaginationRequest pagination)
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery(pagination, new FiltterCustomersDTO { customerType = enCustomerType.Renter.ToString()}));
            return Ok(customers);
        }


        /// <summary>
        /// Get all owners
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>List of owners</returns>
        [HttpGet("Owners")]
        public async Task<IActionResult> GetOwners([FromQuery] PaginationRequest pagination)
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery(pagination, new FiltterCustomersDTO { customerType = enCustomerType.Owner.ToString() }));
            return Ok(customers);
        }


        /// <summary>
        /// Get a specific customer by ID
        /// </summary>
        /// <param name="CustomerId">The customer's unique identifier</param>
        /// <returns>Customer details</returns>
        [HttpGet("{CustomerId}")]
        public async Task<IActionResult> GetCustomerById(Guid CustomerId)
        {
            var response = await _mediator.Send(new GetCustomerByIdQuery(CustomerId));
            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return Ok(response.Data);
        }



        /// <summary>
        /// Get customer by national ID
        /// </summary>
        /// <param name="NationalId">The customer's national ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("NationalId/{NationalId}")]
        public async Task<IActionResult> GetCustomerByNationalId(string NationalId)
        {
            var results = await _mediator.Send(new GetCustomerNationalIdIdQuery(NationalId));
            return results.ToActionResult();
        }


        /// <summary>
        /// Check if a customer exists with the given national ID
        /// </summary>
        /// <param name="nationalId">The national ID to check</param>
        /// <returns>Boolean indicating existence</returns>
        [HttpGet("Exists/NationalId/{nationalId}")]
        public async Task<IActionResult> CheckCustomerExists(string nationalId)
        {
            var exists = await _mediator.Send(new ExistsCustomerExistsQuery(nationalId));
            return Ok(new { Exists = exists });
        }


        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="command">Customer creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The ID of the newly created customer</returns>
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCustomer([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return CreatedAtAction(nameof(GetCustomerById), new { CustomerId = response.Data }, new { CustomerId = response.Data });


        }


        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="customerId">The ID of the customer to update</param>
        /// <param name="command">Updated customer data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{customerId}")]
        public async Task<ActionResult<Guid>> UpdateCustomer([FromRoute] Guid customerId, [FromForm] UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command with { CustomerId = customerId });

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent(); 
        }


        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="CustomerId">The ID of the customer to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{CustomerId}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] Guid CustomerId)
        {
            var response = await _mediator.Send(new DeleteCustomerCommand(CustomerId));

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();
        }

        /// <summary>
        /// Get the total count of customers
        /// </summary>
        /// <returns>Total customer count</returns>
        [HttpGet("Count")]
        public async Task<IActionResult> GetCustomersCount()
        {
            var count = await _mediator.Send(new GetCustomersCountQuery());
            return Ok(new
            {
                TotalCountCustomers = count
            });
        }
    }
}
