using MediatR;
using RealEstate.Application.Common.Errors;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Customer;
using RealEstate.Application.Dtos.ResponseDTO;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Features.Customers.Querys
{
    public class GetCustomerTransactionsQuery : IRequest<AppResponse<List<CustomerTransactionDto>>> 
    {
        public Guid CustomerId { get; }
        public GetCustomerTransactionsQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

    }


    class GetCustomerTransactionsQueryHandler : IRequestHandler<GetCustomerTransactionsQuery, AppResponse<List<CustomerTransactionDto>>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISalesRepository _salesRepository;
        private readonly IRentalsRepository _rentalsRepository;

        public GetCustomerTransactionsQueryHandler(
            
            ICustomerRepository customerRepository,
            ISalesRepository salesRepository,
            IRentalsRepository rentalsRepository
            )
        {
            this._customerRepository = customerRepository;
            this._salesRepository = salesRepository;
            this._rentalsRepository = rentalsRepository;
        }
        public async Task<AppResponse<List<CustomerTransactionDto>>> Handle(GetCustomerTransactionsQuery request, CancellationToken cancellationToken)
        {

            if (!_customerRepository.IsCustomerExists(request.CustomerId))
            {
                return AppResponse<List<CustomerTransactionDto>>.Fail(new NotFoundError("customer", "customerId", request.CustomerId.ToString(), Domain.Enums.enApiErrorCode.CustomerNotFound));
            }
            var transactions = new List<CustomerTransactionDto>();

            var saleTransactions = await getSaleTransactionsAsync(request.CustomerId);
            var BuyTransactions = await GetBuyTransactionsAsync(request.CustomerId);
            var RentTransactions = await GetRentTransactionsAsync(request.CustomerId);
            var LeaseTransactions = await GetLeaseTransactionsAsync(request.CustomerId);

            transactions.AddRange(saleTransactions);
            transactions.AddRange(BuyTransactions);
            transactions.AddRange(RentTransactions);
            transactions.AddRange(LeaseTransactions);
            return AppResponse<List<CustomerTransactionDto>>.Success(transactions);
        }




        private async Task<List<CustomerTransactionDto>> getSaleTransactionsAsync(Guid customerId)
        {

            List<CustomerTransactionDto> saleTransactions = new List<CustomerTransactionDto>();

            var salesList = await  _salesRepository.GetAllAsync(1, 1000000, filter: s => s.SellerId == customerId,includes:s => s.Property);

            foreach (var t in salesList)
            {
                saleTransactions.Add(new CustomerTransactionDto
                {
                    CustomerId = customerId,
                    Amount = t.Price,
                    propertyNumber = t.Property.PropertyNumber,
                    PropertyId = t.PropertyId,
                    TransactionDate = t.SaleDate.ToShortDateString(),
                    Notes = t.Description,
                    TransactionType = ((int)TransactionType.Sale).ToString()
                });
            }

            return saleTransactions;
        }
        private async Task<List<CustomerTransactionDto>> GetBuyTransactionsAsync(Guid customerId)
        {

            List<CustomerTransactionDto> BuyTransactions = new List<CustomerTransactionDto>();

            var buyList = await _salesRepository.GetAllAsync(1, 1000000, filter: s => s.BuyerId == customerId, includes: s => s.Property);

            foreach (var t in buyList)
            {
                BuyTransactions.Add(new CustomerTransactionDto
                {
                    CustomerId = customerId,
                    Amount = t.Price,
                    propertyNumber = t.Property.PropertyNumber,
                    PropertyId = t.PropertyId,
                    TransactionDate = t.SaleDate.ToShortDateString(),
                    Notes = t.Description,
                    TransactionType = ((int)TransactionType.Buy).ToString()
                });
            }

            return BuyTransactions;
        }

        private async Task<List<CustomerTransactionDto>> GetRentTransactionsAsync(Guid customerId)
        {

            List<CustomerTransactionDto> RentTransactions = new List<CustomerTransactionDto>();

            var RentList = await _rentalsRepository.GetAllAsync(1, 1000000, filter: s => s.LessorId == customerId, includes: s => s.Property);

            foreach (var t in RentList)
            {
                RentTransactions.Add(new CustomerTransactionDto
                {
                    CustomerId = customerId,
                    Amount = t.GetTotalPrice(),
                    propertyNumber = t.Property.PropertyNumber,
                    PropertyId = t.PropertyId,
                    TransactionDate = t.CreatedDate.Date.ToShortDateString(),
                    Notes = t.Description,
                    TransactionType = ((int)TransactionType.Rent).ToString()
                });
            }

            return RentTransactions;
        }
        private async Task<List<CustomerTransactionDto>> GetLeaseTransactionsAsync(Guid customerId)
        {

            List<CustomerTransactionDto> LeaseTransactions = new List<CustomerTransactionDto>();

            var LeaseList = await _rentalsRepository.GetAllAsync(1, 1000000, filter: s => s.LesseeId == customerId, includes: s => s.Property);

            foreach (var t in LeaseList)
            {
                LeaseTransactions.Add(new CustomerTransactionDto
                {
                    CustomerId = customerId,
                    Amount = t.GetTotalPrice(),
                    PropertyId = t.PropertyId,
                    propertyNumber = t.Property.PropertyNumber,
                    TransactionDate = t.CreatedDate.Date.ToShortDateString(),
                    Notes = t.Description,
                    TransactionType = ((int)TransactionType.Lease).ToString()
                });
            }

            return LeaseTransactions;
        }

    }
}
