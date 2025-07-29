using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Features.Comments.Commands.Update;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Comments.Commands.Delete
{
    public class DeleteCommentCommand : IRequest<AppResponse>
    {

        public DeleteCommentCommand(Guid? commentId)
        {
            CommentId = commentId;
        }

        public Guid? CommentId { get; }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, AppResponse>
    {
        private readonly ICommantsRepository _commantsRepository; 

        public DeleteCommentCommandHandler(ICommantsRepository commantsRepository)
        {
            this._commantsRepository = commantsRepository;
        }


        public async Task<AppResponse> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {

            if (!request.CommentId.HasValue)
            {
                return AppResponse.Fail(new ValidationError("CommentId", "User Id Is Required", enApiErrorCode.RequiredField));
            }

            var comment = await _commantsRepository.FirstOrDefaultAsync(filter: c => c.Id == request.CommentId.Value && !c.IsDeleted);

            if (comment == null)
                return AppResponse.Fail(new NotFoundError("CommentId", "comment not found", request.CommentId.Value.ToString(), enApiErrorCode.CommentNotFound));


            try
            {
                _commantsRepository.Delete(comment);
                await _commantsRepository.SaveChangesAsync();
            }
            catch 
            {
                return AppResponse.Fail(new InternalServerError("Updating", "failed to Updating a Comment please try again", enApiErrorCode.GeneralError));
            }
            return AppResponse.Success();
        }


        
    }
}
