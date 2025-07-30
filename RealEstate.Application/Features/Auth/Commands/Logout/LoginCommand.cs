using MediatR;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.ResponseDTO;

namespace RealEstate.Application.Features.Auth.Commands.Login
{
    public class LogoutCommand : IRequest<AppResponse<string>>
    { }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, AppResponse<string>>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AppResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync();  
            return AppResponse<string>.Success("Logout successful");
        }
    }
}