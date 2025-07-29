using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Features.Comments.Commands.Create;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Comments.Commands.Update
{
    public class UpdateCommentCommand : IRequest<AppResponse>
    {

        public UpdateCommentCommand(Guid? commentId,string? commentText)
        {
            CommentId = commentId;
            CommentText = commentText;
        }

        public Guid? CommentId { get; }
        public string? CommentText { get; }
    }



    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, AppResponse>
    {
        private readonly ICommantsRepository _commantsRepository; 
        private readonly ICurrentUserService _user;

        public UpdateCommentCommandHandler(
            ICommantsRepository commantsRepository, 
            ICurrentUserService user
            )
        {
            this._commantsRepository = commantsRepository; 
            this._user = user;
        }


        public async Task<AppResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }



            var comment = await _commantsRepository.FirstOrDefaultAsync(filter:c => c.Id == request.CommentId.Value && !c.IsDeleted);

            if (comment == null)
                return AppResponse.Fail(new NotFoundError("CommentId", "comment not found", request.CommentId.Value.ToString(), enApiErrorCode.CommentNotFound));

            comment.CommentText = request.CommentText!;

            try
            {
                await _commantsRepository.UpdateAsync(comment);
                await _commantsRepository.SaveChangesAsync();
            }
            catch  
            {
                return AppResponse.Fail(new InternalServerError("Updating", "failed to Updating a Comment please try again", enApiErrorCode.GeneralError));
            }
            return AppResponse.Success();
        }


        private Result _ValideteRentalData(UpdateCommentCommand request)
        {
            List<Error> errors = new List<Error>();

            if (!_user.UserId.HasValue)
            {
                return Result.Fail(new ValidationError("UserID", "User Id Is Required", enApiErrorCode.RequiredField));
            } 
            if (string.IsNullOrEmpty(request.CommentText))
            {
                errors.Add(new NotFoundError("Comment", "CommentText", "Comment Text Is required", enApiErrorCode.RequiredField));
            }


            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }
    }
}
