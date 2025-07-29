using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Pagination;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Application.Dtos.Users;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetAllCustomersQuery : IRequest<PaginationResponse<CustomerDTO>>
    {
        public PaginationRequest pagination;
        public FiltterCustomersDTO Filtter { get; }

        public GetAllCustomersQuery(PaginationRequest pagination, FiltterCustomersDTO filtter)
        {
            this.pagination = pagination;
            Filtter = filtter;
        }
    }



    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, PaginationResponse<CustomerDTO>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetAllCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResponse<CustomerDTO>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Customer, bool>> filter = Customer =>
                (string.IsNullOrEmpty(request.Filtter.fullName) || Customer.Person.FullName.StartsWith(request.Filtter.fullName)) &&
                (string.IsNullOrEmpty(request.Filtter.gender) || Customer.Person.Gender.ToString() == request.Filtter.gender) &&
                (string.IsNullOrEmpty(request.Filtter.nationalId) || Customer.Person.NationalId == request.Filtter.nationalId) &&
                (string.IsNullOrEmpty(request.Filtter.customerType) || Customer.CustomerType.ToString() == request.Filtter.customerType) &&
                (string.IsNullOrEmpty(request.Filtter.phoneNumber) || Customer.PhoneNumber == request.Filtter.phoneNumber)

                && Customer.IsDeleted == false;

            var Customers = await _customerRepository.GetAllAsync(request.pagination.PageNumber, request.pagination.PageSize, filter, includes: c => c.Person);

            var totalCount = await _customerRepository.CountAsync();

            var response = new PaginationResponse<CustomerDTO>
            {
                Items = _mapper.Map<List<CustomerDTO>>(Customers),
                PageNumber = request.pagination.PageNumber,
                PageSize = request.pagination.PageSize,
                TotalCount = totalCount

            };
            return response;
        }
    }

}
