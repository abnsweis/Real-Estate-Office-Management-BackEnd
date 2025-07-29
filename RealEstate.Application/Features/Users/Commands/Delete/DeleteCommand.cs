using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Commands.Delete
{

    public record DeleteUserCommand(Guid UserId) : IRequest<AppResponse>;

    class DeleteCommandHandler : IRequestHandler<DeleteUserCommand, AppResponse>
    {
        private readonly IUserRepository _userRepository;

        public DeleteCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<AppResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return _userRepository.Delete(request.UserId);
        }
    }

}
