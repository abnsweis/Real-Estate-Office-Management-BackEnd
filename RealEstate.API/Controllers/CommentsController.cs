using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.Category;
using RealEstate.Application.Dtos.Comments;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Application.Features.Categories.Commands;
using RealEstate.Application.Features.Comments.Commands.Create;
using RealEstate.Application.Features.Comments.Commands.Delete;
using RealEstate.Application.Features.Comments.Commands.Update;
using RealEstate.Application.Features.Comments.Querys;
using RealEstate.Application.Features.Ratings.Commands.Create;
using RealEstate.Application.Features.Ratings.Querys;

namespace RealEstate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {

        private readonly ISender _mediator;

        public CommentsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createComment)
        {
 
            var command = new CreateCommentCommand(createComment.PropertyId, createComment.Text);
            var response = await _mediator.Send(command);
            return response.Result.IsFailed ? response.Result.ToActionResult() : CreatedAtAction(nameof(GetPropertyComments), new { propertyId = createComment.PropertyId }, new { propertyId = createComment.PropertyId });
        }

        [HttpGet("property/Id/{propertyId}")]
        public async Task<IActionResult> GetPropertyComments([FromQuery] PaginationRequest pagination, [FromRoute] Guid? propertyId)
        {
            var response = await _mediator.Send(new GetPropertyCommentsQuery(pagination, propertyId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : Ok(response.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto commentDto)
        {
            if (ModelState.IsValid)
            {

            }
            var response = await _mediator.Send(new UpdateCommentCommand(commentDto.CommentId, commentDto.Text));
            return response.Result.IsFailed ? response.Result.ToActionResult() : NoContent();
        }


        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid? commentId)
        {


            var response = await _mediator.Send(new DeleteCommentCommand(commentId));
            return response.Result.IsFailed ? response.Result.ToActionResult() : NoContent();
        }
    }
}
