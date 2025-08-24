using AutoMapper;
using FluentResults;
using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;
namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetCustomerByNationalIdQuery : IRequest<AppResponse<CustomerDTO>>
    {
        public string NationalId { get; }
        public GetCustomerByNationalIdQuery(string nationalId) => NationalId = nationalId;

    }


    public class GetCustomerNationalIdIdQueryHandler : IRequestHandler<GetCustomerByNationalIdQuery, AppResponse<CustomerDTO>>
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

        public async Task<AppResponse<CustomerDTO>> Handle(GetCustomerByNationalIdQuery request, CancellationToken cancellationToken)
        { 
            var customer = await _customerRepository.FirstOrDefaultAsync(
            filter: u => u.Person.NationalId == request.NationalId,
             includes: new Expression<Func<Customer, object>>[]
            {
                            c => c.Person,
                            c => c.Properties,
            });
            if (customer is null)
            {

                return new AppResponse<CustomerDTO>
                {

                    Result = Result.Fail(new NotFoundError("customer", "NationalId", request.NationalId, Domain.Enums.enApiErrorCode.CustomerNotFound))
                };
            }
            var customerDTO = _mapper.Map<CustomerDTO>(customer);
            customerDTO.isBuyer = await _customerRepository.CustomerIsBuyer(customer.Id);
            customerDTO.isOwner = await _customerRepository.CustomerIsOwner(customer.Id);
            customerDTO.IsRenter = await _customerRepository.CustomerIsRenter(customer.Id);
            var ContractsCount = await _customerRepository.GetCustomerContractsCount(customer.Id);

            
            customerDTO.ContractsCount = ContractsCount.ToString();
            return AppResponse<CustomerDTO>.Success(customerDTO);
        }
    }
}
