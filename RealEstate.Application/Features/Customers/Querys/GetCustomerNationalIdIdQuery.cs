using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetCustomerByNationalIdQuery : IRequest<Result<CustomerDTO>>
    {
        public string NationalId { get; }
        public GetCustomerByNationalIdQuery(string nationalId) => NationalId = nationalId;

    }


    public class GetCustomerNationalIdIdQueryHandler : IRequestHandler<GetCustomerByNationalIdQuery, Result<CustomerDTO>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public GetCustomerNationalIdIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper, IFileManager fileManager)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            this._fileManager = fileManager;
        }

        public async Task<Result<CustomerDTO>> Handle(GetCustomerByNationalIdQuery request, CancellationToken cancellationToken)
        {
            var Customer = await _customerRepository.FirstOrDefaultAsync(filter: u => u.Person.NationalId == request.NationalId, includes: x => x.Person);

            Customer.Person.ImageURL = _fileManager.GetPublicURL(Customer.Person.ImageURL);

            if (Customer is null) return Result.Fail(new NotFoundError("Customer", "NationalId", request.NationalId, Domain.Enums.enApiErrorCode.CustomerNotFound));
            return Result.Ok(_mapper.Map<CustomerDTO>(Customer));
        }
    }
}
