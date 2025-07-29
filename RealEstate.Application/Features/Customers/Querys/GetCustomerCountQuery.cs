using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetCustomersCountQuery : IRequest<int> { }

    public class GetCustomerCountQueryHandler : IRequestHandler<GetCustomersCountQuery, int>
    {

        private readonly ICustomerRepository _customerRepository;

        public GetCustomerCountQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<int> Handle(GetCustomersCountQuery request, CancellationToken cancellationToken)
        {
            return await _customerRepository.CountAsync(user => user.IsDeleted == false);
        }
    }

}
