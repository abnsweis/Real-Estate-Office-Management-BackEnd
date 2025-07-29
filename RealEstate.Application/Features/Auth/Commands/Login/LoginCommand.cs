using MediatR;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Dtos.Auth;
using RealEstate.Application.Dtos.ResponseDTO;

namespace RealEstate.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AppResponse<string>>
    {
        public LoginDto LoginInfo { get; }

        public LoginCommand(LoginDto loginInfo) {
            LoginInfo = loginInfo;
        }

    }


    public class LoginCommandHandler : IRequestHandler<LoginCommand, AppResponse<string>>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            this._authService = authService;
        }

        public async Task<AppResponse<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var results = await _authService.Login(request.LoginInfo);

            if (results.IsFailed)
            {
                return AppResponse<string>.Fail(results.Errors);
            }

            return AppResponse<string>.Success(results.Value);
        }
    }
}
