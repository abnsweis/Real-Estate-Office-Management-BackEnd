using FluentResults.Extensions.AspNetCore;
using FluentValidation;
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
/// Controller for managing real estate property operations including
/// creation, retrieval, updating, and deletion of properties
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PropertiesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<UpdatePropertyCommand> _validator;

    /// <summary>
    /// Initializes a new instance of the PropertiesController
    /// </summary>
    /// <param name="mediator">Mediator for handling CQRS patterns</param>
    /// <param name="validator">Validator for update property commands</param>
    public PropertiesController(IMediator mediator, IValidator<UpdatePropertyCommand> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    /// <summary>
    /// Retrieves all properties with optional filtering and pagination
    /// </summary>
    /// <param name="pagination">Pagination configuration (page number and size)</param>
    /// <param name="filter">Optional filters for property search</param>
    /// <returns>Paginated list of property DTOs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProperties([FromQuery] PaginationRequest pagination, [FromQuery] FilterPropertiesDTO filter)
    {
        var response = await _mediator.Send(new GetAllPropertiesQuery(pagination, filter));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves the top 7 featured properties
    /// </summary>
    /// <returns>List of featured property DTOs</returns>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFeaturedPropertiesTop7()
    {
        var response = await _mediator.Send(new GetFeaturedPropertiesTop7Query());
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves the latest 7 added properties
    /// </summary>
    /// <returns>List of recently added property DTOs</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLatestTop7Properties()
    {
        var response = await _mediator.Send(new GetLatestTop7PropertiesQuery());
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves a property by its unique identifier
    /// </summary>
    /// <param name="propertyId">The GUID of the property to retrieve</param>
    /// <returns>Property DTO if found</returns>
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
    /// Retrieves a property by its property number
    /// </summary>
    /// <param name="propertyNumber">The unique property number</param>
    /// <returns>Property DTO if found</returns>
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
    /// Retrieves properties owned by a specific owner using national ID
    /// </summary>
    /// <param name="pagination">Pagination configuration</param>
    /// <param name="nationalId">Owner's national identification number</param>
    /// <returns>Paginated list of owner's properties</returns>
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
    /// Retrieves properties owned by a specific owner using owner ID
    /// </summary>
    /// <param name="OwnerId">The GUID of the property owner</param>
    /// <param name="pagination">Pagination configuration</param>
    /// <returns>Paginated list of owner's properties</returns>
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
    /// Retrieves properties by category ID
    /// </summary>
    /// <param name="CategoryId">The GUID of the property category</param>
    /// <param name="pagination">Pagination configuration</param>
    /// <returns>Paginated list of properties in category</returns>
    [HttpGet("Category/{CategoryId}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesByCategory(Guid CategoryId, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByCategoryQuery(pagination, CategoryId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties by status (e.g., "ForSale", "Rented")
    /// </summary>
    /// <param name="status">The status filter value</param>
    /// <param name="pagination">Pagination configuration</param>
    /// <returns>Paginated list of properties with matching status</returns>
    [HttpGet("Status/{status}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesByStatus(string status, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByStatusQuery(pagination, status));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves properties by location
    /// </summary>
    /// <param name="location">The location search string</param>
    /// <param name="pagination">Pagination configuration</param>
    /// <returns>Paginated list of properties in location</returns>
    [HttpGet("Location/{location}")]
    [ProducesResponseType(typeof(PaginationResponse<PropertyDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesByLocation(string location, [FromQuery] PaginationRequest pagination)
    {
        var response = await _mediator.Send(new GetPropertiesByLocationQuery(pagination, location));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Retrieves the total count of properties
    /// </summary>
    /// <returns>Object containing the count of properties</returns>
    [HttpGet("count")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertiesCount()
    {
        var propertiesCount = await _mediator.Send(new GetPropertiesCountQuery());
        return Ok(new { Count = propertiesCount });
    }

    /// <summary>
    /// Retrieves properties within a specified price range
    /// </summary>
    /// <param name="minPrice">Minimum price filter (optional)</param>
    /// <param name="maxPrice">Maximum price filter (optional)</param>
    /// <param name="pagination">Pagination configuration</param>
    /// <returns>Paginated list of properties in price range</returns>
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
    /// Creates a new property listing
    /// </summary>
    /// <param name="propertyData">Property creation data transfer object</param>
    /// <returns>Created response with new property ID</returns>
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
    /// Updates an existing property listing
    /// </summary>
    /// <param name="propertyId">The GUID of the property to update</param>
    /// <param name="propertyData">Property update data transfer object</param>
    /// <returns>No content response if successful</returns>
    [HttpPut("Update/{propertyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
    /// Updates a property's status to "Rented"
    /// </summary>
    /// <param name="propertyId">The GUID of the property to update</param>
    /// <returns>Updated property DTO</returns>
    [HttpPut("Update/Status/Rented/{propertyId}")]
    [ProducesResponseType(typeof(PropertyDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertyToRented(Guid propertyId)
    {
        var response = await _mediator.Send(new UpdatePropertyStatusToCommand("Rented", propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Updates a property's status to "Sold"
    /// </summary>
    /// <param name="propertyId">The GUID of the property to update</param>
    /// <returns>Updated property DTO</returns>
    [HttpPut("Update/Status/Sold/{propertyId}")]
    [ProducesResponseType(typeof(PropertyDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertyToSold(Guid propertyId)
    {
        var response = await _mediator.Send(new UpdatePropertyStatusToCommand("Sold", propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }

    /// <summary>
    /// Deletes a property by its ID
    /// </summary>
    /// <param name="propertyId">The GUID of the property to delete</param>
    /// <returns>Deleted property ID</returns>
    [HttpDelete("Delete/Id/{propertyId}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProperty(Guid propertyId)
    {
        var response = await _mediator.Send(new DeletePropertyCommand(propertyId));
        return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
    }
}