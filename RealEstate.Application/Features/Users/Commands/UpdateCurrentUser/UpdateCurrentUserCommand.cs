using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Users;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Commands.UpdateCurrentUser
{
    public class UpdateCurrentUserCommand : IRequest<AppResponse>
    {
        public UpdateCurrentUserDto Data { get; set; }

        public UpdateCurrentUserCommand(UpdateCurrentUserDto data)
        {
            Data = data;
        }
    }


    public class UpdateCurrentUserCommandHandler : IRequestHandler<UpdateCurrentUserCommand, AppResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileManager _fileManager;
        private readonly ICurrentUserService _currentUserService;
        private string _Imagepath = string.Empty;

        public UpdateCurrentUserCommandHandler(
            IUserRepository userRepository,
            IFileManager fileManager,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _fileManager = fileManager;
            _currentUserService = currentUserService;
        }

        public async Task<AppResponse> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
        {
            List<Error> errors = new List<Error>();

            var userId = _currentUserService.UserId;
            var user = await _userRepository.GetByIdAsync(userId??Guid.Empty);

            if (user is null)
            {
                errors.Add(new NotFoundError("user", "userId", nameof(userId), enApiErrorCode.UserNotFound));
                return new AppResponse { Result = Result.Fail(errors) };
            }

            // Check Email
            if (_userRepository.IsEmailAlreadyTaken(request.Data.Email) && request.Data.Email != user.Email)
            {
                errors.Add(new ValidationError(nameof(request.Data.Email), "Email Already Taken", enApiErrorCode.EmailAlreadyTaken));
            }

            // Check Username
            if (_userRepository.IsUsernameAlreadyTaken(request.Data.Username) && request.Data.Username != user.UserName)
            {
                errors.Add(new ValidationError(nameof(request.Data.Username), "Username Already Taken", enApiErrorCode.UsernameAlreadyTaken));
            }

            // Check Phone Number
            if (_userRepository.IsPhoneNumberAlreadyTaken(request.Data.PhoneNumber) && request.Data.PhoneNumber != user.PhoneNumber)
            {
                errors.Add(new ValidationError(nameof(request.Data.PhoneNumber), "Phone Number Already Taken", enApiErrorCode.PhoneAlreadyTaken));
            }

            // Handle Image Upload
            if (request.Data.ImageURL != null)
            {
                var result = await _fileManager.SaveUserProfileImageAsync(request.Data.ImageURL);
                if (result.IsSuccess)
                {
                    _Imagepath = result.Value;
                } else
                {
                    errors.AddRange(result.Errors.Cast<Error>());
                }
            } else
            {
                _Imagepath = user.Person.ImageURL;
            }

            if (errors.Any())
            {
                _fileManager.DeleteFile(_Imagepath);
                return new AppResponse { Result = Result.Fail(errors) };
            }

            // Update User Data
            user.Person.FullName = request.Data.FullName;
            user.Person.Gender = request.Data.Gender;
            user.Person.DateOfBirth = request.Data.DateOfBirth.Value;
            user.Email = request.Data.Email;
            user.PhoneNumber = request.Data.PhoneNumber;
            user.UserName = request.Data.Username;
            user.Person.ImageURL = _Imagepath;

            var Response = await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return Response;
        }
    }

}
