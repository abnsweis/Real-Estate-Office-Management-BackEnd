using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Users;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Features.Users.Commands.Create
{
    public class CreateUserCommand : IRequest<AppResponse<Guid>>
    {
        public CreateUserDto Data { get; set; }

        public CreateUserCommand(CreateUserDto user)
        {
            Data = user;
        }
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, AppResponse<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileManager _fileManager;
        private string _Imagepath = string.Empty;
        public CreateUserCommandHandler(IUserRepository userRepository, IFileManager fileManager)
        {
            _userRepository = userRepository;
            _fileManager = fileManager;
        }


        public async Task<AppResponse<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            List<Error> errors = new List<Error>();

            if (_userRepository.IsEmailAlreadyTaken(request.Data.Email))
            {
                errors.Add(new ValidationError(nameof(request.Data.Email), "Email Already Taken", enApiErrorCode.EmailAlreadyTaken));
            }

            if (_userRepository.IsUsernameAlreadyTaken(request.Data.Username))
            {
                errors.Add(new ValidationError(nameof(request.Data.Username), "Username Already Taken", enApiErrorCode.UsernameAlreadyTaken));
            }

            if (_userRepository.IsPhoneNumberAlreadyTaken(request.Data.PhoneNumber))
            {
                errors.Add(new ValidationError(nameof(request.Data.PhoneNumber), "Phone Number Already Taken", enApiErrorCode.PhoneAlreadyTaken));
            }

            if (request.Data.ImageURL is null)
            {
                var result = _fileManager.SetDefaultUserProfileImage();
                if (result.IsSuccess)
                {
                    _Imagepath = result.Value;
                } else
                {
                    errors.AddRange(result.Errors.Cast<Error>());
                }
            } else
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
                return new AppResponse<Guid> { Result = Result.Fail(errors) };
            }



            var user = new UserDomain
            {
                UserName = request.Data.Username,
                Email = request.Data.Email,
                Password = request.Data.Password,
                PhoneNumber = request.Data.PhoneNumber,
                Person = new Person
                {
                    FullName = request.Data.FullName,
                    DateOfBirth = request.Data.DateOfBirth.Value,
                    Gender = request.Data.Gender,
                    ImageURL = _Imagepath,
                }
            };



            var Response = await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();


            return Response;
        }


    }

}
