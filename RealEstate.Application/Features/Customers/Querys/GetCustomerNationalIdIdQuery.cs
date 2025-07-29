using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetCustomerNationalIdIdQuery : IRequest<Result<CustomerDTO>>
    {
        public string NationalId { get; }
        public GetCustomerNationalIdIdQuery(string nationalId) => NationalId = nationalId;

    }


    public class GetCustomerNationalIdIdQueryHandler : IRequestHandler<GetCustomerNationalIdIdQuery, Result<CustomerDTO>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetCustomerNationalIdIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<Result<CustomerDTO>> Handle(GetCustomerNationalIdIdQuery request, CancellationToken cancellationToken)
        {
            var Customer = await _customerRepository.FirstOrDefaultAsync(filter: u => u.Person.NationalId == request.NationalId, includes: x => x.Person);
            if (Customer is null) return Result.Fail(new NotFoundError("Customer", "NationalId", request.NationalId, Domain.Enums.enApiErrorCode.CustomerNotFound));
            return Result.Ok(_mapper.Map<CustomerDTO>(Customer));
        }
    }
}
