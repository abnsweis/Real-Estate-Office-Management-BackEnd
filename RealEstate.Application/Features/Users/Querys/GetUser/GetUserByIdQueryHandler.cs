using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Querys.GetUser
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper, IFileManager fileManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            this._fileManager = fileManager;
        }

        public async Task<Result<UserDTO>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(filter: u => u.Id == request.Id, Includes: x => x.Person);
            if (user is null) return Result.Fail(new NotFoundError("user", "userid", request.Id.ToString(), Domain.Enums.enApiErrorCode.UserNotFound));

            user.Person.ImageURL = _fileManager.GetPublicURL(user.Person.ImageURL);

            return Result.Ok(_mapper.Map<UserDTO>(user));

        }
    }

    public class GetUserCountQueryHandler : IRequestHandler<GetUserCountQuery, int>
    {
        private readonly IUserRepository _userRepository;

        public GetUserCountQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(GetUserCountQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.CountAsync();
        }
    }
}
