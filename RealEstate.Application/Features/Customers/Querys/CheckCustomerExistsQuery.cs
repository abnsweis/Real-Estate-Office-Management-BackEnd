using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Features.Users.Querys.Check;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class ExistsCustomerExistsQuery : IRequest<bool>
    {
        public string NationalId { get; }

        public ExistsCustomerExistsQuery(string nationalId)
        {
            NationalId = nationalId;
        }

    }



    public class CheckCustomerExistsQueryHandler : IRequestHandler<ExistsCustomerExistsQuery, bool>
    {
        private readonly ICustomerRepository _customerRepository;

        public CheckCustomerExistsQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Task<bool> Handle(ExistsCustomerExistsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_customerRepository.IsCustomerExists(request.NationalId));
        }
    }
}
