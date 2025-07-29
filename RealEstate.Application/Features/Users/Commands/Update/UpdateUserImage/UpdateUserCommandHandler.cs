using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Users.Commands.Update.UpdateUserImage
{
    public class UpdateUserImageCommandHandler : IRequestHandler<UpdateUserImageCommand, AppResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileManager _fileManager;
        private string _Imagepath = string.Empty;
        public UpdateUserImageCommandHandler(IUserRepository userRepository, IFileManager fileManager)
        {
            _userRepository = userRepository;
            _fileManager = fileManager;
        }

        public async Task<AppResponse> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                return new AppResponse { Result = Result.Fail(new NotFoundError("user", "userid", request.UserId.ToString(), enApiErrorCode.UserNotFound)), Data = request.UserId };
            }


            if (request.ImageFile == null)
            {
                return new AppResponse { Result = Result.Fail(new BadRequestError("UserProfileImage", "Image File Is Requierd", enApiErrorCode.MissingUploadedFile)), Data = request.ImageFile };
            }


            var result = await _fileManager.SaveUserProfileImageAsync(request.ImageFile);

            if (result.IsSuccess)
            {
                _Imagepath = result.Value;
            } else
            {
                return new AppResponse { Result = Result.Fail(result.Errors.Cast<Error>()) };
            }

            var OldImage = user.Person.ImageURL;
            user.Person.ImageURL = _Imagepath;

            var response = await _userRepository.UpdateAsync(user);

            if (response.Result.IsSuccess)
            {
                var DeleteResults = _fileManager.DeleteFile(OldImage);
                if (DeleteResults.IsFailed)
                {
                    return new AppResponse { Result = Result.Fail(DeleteResults.Errors.Cast<Error>()) };
                }
            }

            return response;
        }
    }

}
