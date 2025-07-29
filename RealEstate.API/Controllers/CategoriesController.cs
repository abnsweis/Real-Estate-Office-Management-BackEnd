using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Features.Categories.Querys;
using FluentResults.Extensions.AspNetCore;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Features.Categories.Commands;
using RealEstate.Domain.Entities;
using RealEstate.Application.Features.Customers.Commands.Update;
using RealEstate.Application.Features.Customers.Commands.Delete;
using RealEstate.Application.Features.Categories.Commands.NewFolder;
using Microsoft.AspNetCore.Authorization;
using Azure;

namespace RealEstate.API.Controllers
{
    /// <summary>
    /// Controller  Categories
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _mediator;

        public CategoriesController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all categories with pagination.
        /// </summary>
        /// <param name="pagination">Pagination parameters (page number and page size).</param>
        /// <param name="CategoryName">Optional category name filter.</param>
        /// <returns>Paginated list of categories.</returns>
        /// <response code="200">Returns the categories list.</response>
        /// <response code="400">If the pagination parameters are invalid.</response>
        [HttpGet]
         // [Authorize(Roles = "SuperAdmin,Admin,User")]All roles can view categories
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] PaginationRequest pagination,
            [FromQuery] string? CategoryName)
        {
            var response = await _mediator.Send(new GetAllCategoriesQuery(pagination, CategoryName));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="categoryId">The unique GUID identifier of the category.</param>
        /// <returns>Category details including properties and subcategories.</returns>
        /// <response code="200">Successfully returned the category details.</response>
        /// <response code="404">Category with specified ID was not found.</response>
        [HttpGet("{categoryId}")]
        [Authorize(Roles = "SuperAdmin,Admin,User")] // All roles can view category details
        public async Task<IActionResult> GetCategoryById(Guid categoryId)
        {
            var response = await _mediator.Send(new GetCategoryByIdQuery(categoryId));
            return response.Result.IsFailed ? response.Result.ToActionResult():  Ok(response.Data);
        }





        /// <summary>
        /// Retrieves a specific category by name.
        /// </summary>
        /// <param name="categoryName">The exact name of the category.</param>
        /// <returns>Category details including properties and subcategories.</returns>
        /// <response code="200">Successfully returned the category details.</response>
        /// <response code="404">Category with specified name was not found.</response>
        [HttpGet("CategoryName/{categoryName}")]
        [Authorize(Roles = "SuperAdmin,Admin,User")] // All roles can search by name
        public async Task<IActionResult> GetCategoryByName(string categoryName)
        {
            var response = await _mediator.Send(new GetCategoryByNameQuery(categoryName));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }



        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="categoryData">Category creation data.</param>
        /// <returns>Returns the ID of the newly created category.</returns>
        /// <response code="201">Category successfully created.</response>
        /// <response code="400">Invalid input data or validation errors.</response>
        /// <response code="409">Category with same name already exists.</response>
        [HttpPost] 
        public async Task<IActionResult> CreateCategory([FromBody]CreateUpdateCategoryDTO categoryData)
        {
            var command = new CreateCategoryCommand(categoryData);
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { categoryId = response.Data },
                new { categoryId = response.Data });
        }


        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="categoryId">The ID of the category to update.</param>
        /// <param name="categoryData">Updated category data.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Category successfully updated.</response>
        /// <response code="400">Invalid input data or validation errors.</response>
        /// <response code="404">Category not found.</response>
        /// <response code="409">Category name already exists (conflict).</response>
        [HttpPut("{categoryId}")]
        [Authorize(Roles = "SuperAdmin,Admin")] // Only admins can update categories
        public async Task<ActionResult<Guid>> UpdateCategory(
            [FromRoute] Guid categoryId,
            [FromBody] CreateUpdateCategoryDTO categoryData)
        {
            var command = new UpdateCategoryCommand(categoryId, categoryData);
            var response = await _mediator.Send(command);

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();
        }





        /// <summary>
        /// Deletes a category by its unique ID (soft delete).
        /// </summary>
        /// <param name="categoryId">The ID of the category to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Category successfully deleted.</response>
        /// <response code="400">Invalid input ID.</response>
        /// <response code="404">Category not found.</response>
        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can delete categories
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid categoryId)
        {
            var response = await _mediator.Send(new DeleteCategoryCommand(categoryId));

            if (response.Result.IsFailed)
            {
                return response.Result.ToActionResult();
            }

            return NoContent();
        }

    }
}