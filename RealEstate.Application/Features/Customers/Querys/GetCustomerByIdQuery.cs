using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetCustomerByIdQuery : IRequest<AppResponse<CustomerDTO>>
    {
        public Guid CustomerId { get; }
        public GetCustomerByIdQuery(Guid customerId) => CustomerId = customerId;

    }


    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, AppResponse<CustomerDTO>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<AppResponse<CustomerDTO>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var Customer = await _customerRepository.FirstOrDefaultAsync(filter: u => u.Id == request.CustomerId, includes: x => x.Person);
            if (Customer is null)
            {
                return new AppResponse<CustomerDTO>
                {

                    Result = Result.Fail(new NotFoundError("customer", "customerId", request.CustomerId.ToString(), Domain.Enums.enApiErrorCode.CustomerNotFound))
                };
            }

            return AppResponse<CustomerDTO>.Success(_mapper.Map<CustomerDTO>(Customer));

        }
    }
}
