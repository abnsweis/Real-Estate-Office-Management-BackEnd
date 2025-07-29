using FluentResults;
using MediatR;
using Newtonsoft.Json;
using RealEstate.Application.Common;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Comments.Commands.Create
{
    public class CreateCommentCommand : IRequest<AppResponse>
    { 
        public Guid? PropertyId { get; }
        public string Text { get; }
        public CreateCommentCommand(Guid? propertId,string Text)
        {
            PropertyId = propertId;
            this.Text = Text;
        }
    }



    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, AppResponse>
    {
        private readonly ICommantsRepository _commantsRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _user;

        public CreateCommentCommandHandler(
            ICommantsRepository commantsRepository,
            IPropertyRepository propertyRepository,
            IUserRepository userRepository,
            ICurrentUserService user
            )
        {
            this._commantsRepository = commantsRepository;
            this._propertyRepository = propertyRepository;
            this._userRepository = userRepository;
            this._user = user;
        }


        public async Task<AppResponse> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _ValideteRentalData(request);

            if (validationResults.IsFailed)
            {
                return AppResponse.Fail(validationResults.Errors);
            }



            var NewComment = new Comment
            {
                CommentText = request.Text,
                PropertyId = request.PropertyId!.Value,
                UserId = _user.UserId!.Value,
                CreatedDate = DateTimeOffset.UtcNow
            };


            try
            {
                await _commantsRepository.AddAsync(NewComment);
                await _commantsRepository.SaveChangesAsync();
            }
            catch 
            {
                return AppResponse.Fail(new InternalServerError("Saving", "failed to save a Comment please try again", enApiErrorCode.GeneralError));
            }
            return AppResponse.Success();
        }

        private Result _ValideteRentalData(CreateCommentCommand request)
        {
            List<Error> errors = new List<Error>();
            
            if (!_user.UserId.HasValue)
            {
                return Result.Fail(new ValidationError("UserID", "User Id Is Required", enApiErrorCode.RequiredField));
            }            
            if (!request.PropertyId.HasValue)
            {
                return Result.Fail(new ValidationError("PropertId", "Propert Id Is Required", enApiErrorCode.RequiredField));
            } 
            if (!_userRepository.IsUserExists(_user.UserId.Value))
            {
                errors.Add(new NotFoundError("User","UserID",_user.UserId.ToString(),enApiErrorCode.UserNotFound));
            }
            if (!_propertyRepository.IsPropertyExistsById(request.PropertyId.Value))
            {
                errors.Add(new NotFoundError("Property", "PropertyID", request.PropertyId.Value.ToString(), enApiErrorCode.UserNotFound));
            }
            if (string.IsNullOrEmpty(request.Text))
            {
                errors.Add(new NotFoundError("Comment", "CommentText", "Comment Text Is required", enApiErrorCode.RequiredField));
            }


            return errors.Any() ? Result.Fail(errors) : Result.Ok();
        }
    }
}
