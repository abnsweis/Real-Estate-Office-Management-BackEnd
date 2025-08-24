using AutoMapper;
using MediatR;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Dtos.CustomerDTO;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetTopLatestCustomersQuery : IRequest<List<CustomerDTO>>
    {
        public int Count { get; }

        public GetTopLatestCustomersQuery(int count = 5)
        {
            Count = count;
        }
    }
    public class GetTopLatestCustomersQueryHandler : IRequestHandler<GetTopLatestCustomersQuery, List<CustomerDTO>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public GetTopLatestCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper,IFileManager fileManager)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            this._fileManager = fileManager;
        }

        public async Task<List<CustomerDTO>> Handle(GetTopLatestCustomersQuery request, CancellationToken cancellationToken)
        { 
            Expression<Func<Customer, bool>> filter = c => c.IsDeleted == false;
             
            var latestCustomers = await _customerRepository.GetAllAsync(
                pageNumber: 1,
                pageSize: request.Count,
                filter: filter,
                orderBy: q => q.OrderByDescending(c => c.CreatedDate),
                  includes: new Expression<Func<Customer, object>>[]
               {
                   c => c.Person,
                   c => c.Properties,
               });

            var customers = _mapper.Map<List<CustomerDTO>>(latestCustomers);

            foreach (var customer in customers)
            {
                customer.ImageURL = _fileManager.GetPublicURL(customer.ImageURL);
                customer.isBuyer = await _customerRepository.CustomerIsBuyer(Guid.Parse(customer.CustomerId));
                customer.isOwner = await _customerRepository.CustomerIsOwner(Guid.Parse(customer.CustomerId));
                customer.IsRenter = await _customerRepository.CustomerIsRenter(Guid.Parse(customer.CustomerId));
            }

            return customers;
        }
    }
}
