using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Users; 
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Users.Commands.Update
{
    public class UpdateUserCommand : IRequest<AppResponse>
    {
        public UpdateUserDto Data { get; set; }
        public UpdateUserCommand(UpdateUserDto user)
        {
            Data = user;
        }
    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, AppResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileManager _fileManager;
        private string _Imagepath = string.Empty;
        public UpdateUserCommandHandler(IUserRepository userRepository, IFileManager fileManager)
        {
            _userRepository = userRepository;
            _fileManager = fileManager;
        }


        public async Task<AppResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            List<Error> errors = new List<Error>();

            var user = await _userRepository.GetByIdAsync(request.Data.UserId);

            if (user is null)
            {
                errors.Add(new NotFoundError("user", "userId", nameof(request.Data.UserId), enApiErrorCode.UserNotFound));
                return new AppResponse { Result = Result.Fail(errors) };
            }


            if (_userRepository.IsEmailAlreadyTaken(request.Data.Email) && request.Data.Email != user.Email)
            {
                errors.Add(new ValidationError(nameof(request.Data.Email), "Email Already Taken", enApiErrorCode.EmailAlreadyTaken));
            }

            if (_userRepository.IsUsernameAlreadyTaken(request.Data.Username) && request.Data.Username != user.UserName)
            {
                errors.Add(new ValidationError(nameof(request.Data.Username), "Username Already Taken", enApiErrorCode.UsernameAlreadyTaken));
            }

            if (_userRepository.IsPhoneNumberAlreadyTaken(request.Data.PhoneNumber) && request.Data.PhoneNumber != user.PhoneNumber)
            {
                errors.Add(new ValidationError(nameof(request.Data.PhoneNumber), "Phone Number Already Taken", enApiErrorCode.PhoneAlreadyTaken));
            }
 

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
            }

            if (errors.Any())
            {
                _fileManager.DeleteFile(_Imagepath);
                return new AppResponse { Result = Result.Fail(errors) };
            }


            user.Person.FullName = request.Data.FullName;
            user.Person.Gender = request.Data.Gender;
            user.Person.DateOfBirth = request.Data.DateOfBirth.Value;
            user.Email = request.Data.Email;
            user.PhoneNumber = request.Data.PhoneNumber;
            user.UserName = request.Data.Username;



            var Response = await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();


            return Response;
        }


    }

}
