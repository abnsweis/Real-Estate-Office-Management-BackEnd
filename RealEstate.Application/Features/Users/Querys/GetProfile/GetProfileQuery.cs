using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Querys.GetUser
{
    public class GetProfileQuery : IRequest<AppResponse<UserDTO>>
    { 
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, AppResponse<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;
        private readonly ICurrentUserService _currentUserService;

        public GetProfileQueryHandler(
            IUserRepository userRepository, 
            IMapper mapper, 
            IFileManager fileManager,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            this._fileManager = fileManager;
            this._currentUserService = currentUserService;
        }

        public async Task<AppResponse<UserDTO>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
                return AppResponse<UserDTO>.Fail(new UnauthorizedError("Invalid User Id "));
            var user = await _userRepository.FirstOrDefaultAsync(
                filter: u => u.Id == userId && !u.Person.IsDeleted,
                Includes: x => x.Person);
            if (user is null) return AppResponse<UserDTO>.Fail(new NotFoundError("user", "userid", userId.ToString() ?? "", Domain.Enums.enApiErrorCode.UserNotFound));

            user.Person.ImageURL = _fileManager.GetPublicURL(user.Person.ImageURL);

            return AppResponse<UserDTO>.Success(_mapper.Map<UserDTO>(user)) ;

        }
    }

 
}
