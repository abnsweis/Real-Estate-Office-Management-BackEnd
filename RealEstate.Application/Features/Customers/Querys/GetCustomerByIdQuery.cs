using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Services;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Entities;
using System.Linq.Expressions;

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
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository,IFileManager fileManager ,IMapper mapper)
        {
            _customerRepository = customerRepository;
            this._fileManager = fileManager;
            _mapper = mapper;
        }

        public async Task<AppResponse<CustomerDTO>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(
                filter: u => u.Id == request.CustomerId,
                 includes: new Expression<Func<Customer, object>>[]
                {
                    c => c.Person,
                    c => c.Properties,
                });
            if (customer is null)
            {

                return new AppResponse<CustomerDTO>
                {

                    Result = Result.Fail(new NotFoundError("customer", "customerId", request.CustomerId.ToString(), Domain.Enums.enApiErrorCode.CustomerNotFound))
                };
            }
            var ContractsCount = await _customerRepository.GetCustomerContractsCount(request.CustomerId);



            var customerDTO = _mapper.Map<CustomerDTO>(customer);
            customerDTO.isBuyer = await _customerRepository.CustomerIsBuyer(customer.Id);
            customerDTO.isOwner = await _customerRepository.CustomerIsOwner(customer.Id);
            customerDTO.IsRenter = await _customerRepository.CustomerIsRenter(customer.Id);
            customerDTO.ImageURL = _fileManager.GetPublicURL(customerDTO.ImageURL);
            customerDTO.ContractsCount = ContractsCount.ToString();
            return AppResponse<CustomerDTO>.Success(customerDTO);

        }
    }
}
