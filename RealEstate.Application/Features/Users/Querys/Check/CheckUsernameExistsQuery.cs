using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Users.Querys.Check
{

    public enum enCheckTo
    {
        Email,
        Username,
        Phone
    }


    public class ExistsUserQuery : IRequest<bool>
    {
        public string checkValue { get; }
        public enCheckTo CheckTo { get; }

        public ExistsUserQuery(string CheckValue, enCheckTo checkTo)
        {
            checkValue = CheckValue;
            CheckTo = checkTo;
        }

    }




    public class CheckUsernameExistsQueryHandler : IRequestHandler<ExistsUserQuery, bool>
    {
        private readonly IUserRepository _userRepository;

        public CheckUsernameExistsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<bool> Handle(ExistsUserQuery request, CancellationToken cancellationToken)
        {
            switch (request.CheckTo)
            {
                case enCheckTo.Username:
                    {
                        return Task.FromResult(_userRepository.IsUsernameAlreadyTaken(request.checkValue));
                    }

                case enCheckTo.Email:
                    {
                        return Task.FromResult(_userRepository.IsEmailAlreadyTaken(request.checkValue));
                    }
                case enCheckTo.Phone:
                    {       
                        return Task.FromResult(_userRepository.IsPhoneNumberAlreadyTaken(request.checkValue));
                    }


                default:
                    return Task.FromResult(false);
            }


        }
    }
}
