using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Property;
using RealEstate.Application.Features.Customers.Commands.Update;
using RealEstate.Application.Features.Properties.Commands.Create;
using RealEstate.Application.Features.Properties.Commands.Delete;
using RealEstate.Application.Features.Properties.Querys;
using RealEstate.Application.Features.Properties.Querys.Filter;
using RealEstate.Application.Features.Properties.Querys.Get;
using RealEstate.Application.Features.Propertys.Commands.Update;

/// <summary>
/// Controller for managing Property operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>
    /// Constructor for injecting dependencies
    /// </summary>
    public PropertiesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all properties with optional filtering and pagination.
    /// </summary>
    /// <param name="pagination">Pagination parameters (PageNumber, PageSize)</param>
    /// <param name="filter">Optional filter parameters</param>
    /// <returns>A paginated list of properties.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProperties([FromQuery] PaginationRequest pagination, [FromQuery] FilterPropertiesDTO filter)
    {
        var response = await _mediator.Send(new GetAllPropertiesQuery(pagination, filter));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }
    [HttpGet("featured")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFeaturedPropertiesTop7()
    {
        var response = await _mediator.Send(new GetFeaturedPropertiesTop7Query());
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }
    /// <summary>
    /// Retrieves a single property by its unique ID.
    /// </summary>
    /// <param name="propertyId">The ID of the property</param>
    /// <returns>The matched property if found</returns>
    [HttpGet("Id/{propertyId:guid}")]
    [ProducesResponseType(typeof(PropertyDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPropertyById(Guid propertyId)
    {
        var response = await _mediator.Send(new GetPropertyByIdQuery(propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves a single property by its property number.
    /// </summary>
    /// <param name="propertyNumber">The property number</param>
    /// <returns>The matched property if found</returns>
    [HttpGet("Number/{propertyNumber}")]
    [ProducesResponseType(typeof(PropertyDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPropertyByNumber(string propertyNumber)
    {
        var response = await _mediator.Send(new GetPropertyByNumberQuery(propertyNumber));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties owned by a specific owner using their national ID.
    /// </summary>
    /// <param name="pagination">Pagination parameters</param>
    /// <param name="nationalId">Owner's national ID</param>
    /// <returns>List of properties owned by the user</returns>
    [HttpGet("Owner/nationalId/{nationalId}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPropertiesByNationalId([FromQuery] PaginationRequest pagination, string? nationalId)
    {
        var response = await _mediator.Send(new GetPropertiesByNationalIdQuery(pagination, nationalId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties owned by a specific owner using their ID.
    /// </summary>
    /// <param name="OwnerId">The owner's unique ID</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>List of owned properties</returns>
    [HttpGet("Owner/Id/{OwnerId}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOwnerPropertiesById(Guid OwnerId, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByOwnerIdQuery(pagination, OwnerId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties by category name.
    /// </summary>
    /// <param name="CategoryName">The name of the category</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>List of properties in the category</returns>
    [HttpGet("Category/{CategoryId}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesByCategory(Guid CategoryId, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByCategoryQuery(pagination, CategoryId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties by their status (e.g., ForSale, Rented, etc.).
    /// </summary>
    /// <param name="status">The status of the property</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>List of properties with the specified status</returns>
    [HttpGet("Status/{status}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesByStatus(string status, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByStatusQuery(pagination, status));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties by location.
    /// </summary>
    /// <param name="location">The location</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>List of properties at the specified location</returns>
    [HttpGet("Location/{location}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesByLocation(string location, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByLocationQuery(pagination, location));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties within a specified price range.
    /// </summary>
    /// <param name="minPrice">Minimum price (optional)</param>
    /// <param name="maxPrice">Maximum price (optional)</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>List of properties matching the price range</returns>
    [HttpGet("PriceRange")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPropertiesByPriceRange(
        [FromQuery] int? minPrice,
        [FromQuery] int? maxPrice,
        [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByPriceRangeQuery(pagination, minPrice, maxPrice));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Creates a new property.
    /// </summary>
    /// <param name="propertyData">The property data to create</param>
    /// <returns>The created property's ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePropertie([FromForm] CreatePropertyDTO propertyData)
    {
        var Command = new CreatePropertyCommand(propertyData);
        var response = await _mediator.Send(Command);

        if (response.Result.IsFailed)
        {
            return response.Result.ToActionResult();
        }

        return CreatedAtAction(
            nameof(GetPropertyById),
            new { propertyId = response.Data },
            new { propertyId = response.Data });
    }

    /// <summary>
    /// Updates an existing property.
    /// </summary>
    /// <param name="propertyId">The ID of the property to update</param>
    /// <param name="propertyData">The updated property data</param>
    /// <returns>The updated property's ID</returns>
    [HttpPut("Update/{propertyId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertie(Guid propertyId, [FromForm] UpdatePropertyDTO propertyData)
    {
        var Command = new UpdatePropertyCommand(propertyData, propertyId);
        var response = await _mediator.Send(Command);

        if (response.Result.IsFailed)
        {
            return response.Result.ToActionResult();
        }

        return NoContent();
    }

    /// <summary>
    /// Updates a property's status to "Rented".
    /// </summary>
    /// <param name="propertyId">The ID of the property to update</param>
    /// <returns>The updated property's ID</returns>
    [HttpPut("Update/Status/Rented/{propertyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertyToRented(Guid propertyId)
    {
        var response = await _mediator.Send(new UpdatePropertyStatusToCommand("Rented", propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Updates a property's status to "Sold".
    /// </summary>
    /// <param name="propertyId">The ID of the property to update</param>
    /// <returns>The updated property's ID</returns>
    [HttpPut("Update/Status/Sold/{propertyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertyToSold(Guid propertyId)
    {
        var response = await _mediator.Send(new UpdatePropertyStatusToCommand("Sold", propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Deletes a property by its ID.
    /// </summary>
    /// <param name="propertyId">The ID of the property to delete</param>
    /// <returns>The deleted property's ID</returns>
    [HttpDelete("Delete/Id/{propertyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProperty(Guid propertyId)
    {
        var response = await _mediator.Send(new DeletePropertyCommand(propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }
}