using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RealEstate.Infrastructure.Repositorios
{
    public class CustomerRepository : Repository<Customer> , ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context) : base(context) {
        
            this._context = context;
        }

        public async Task<int> CountCreatedInCurrentMonth()
        {
            var now = DateTimeOffset.Now;  

            var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
            var endOfMonth = startOfMonth.AddMonths(1);

            return await _context.Customers
                .CountAsync(c => c.CreatedDate >= startOfMonth && c.CreatedDate < endOfMonth);
        }

        public async Task<bool> CustomerIsBuyer(Guid customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerType == CustomerType.Buyer && c.Id == customerId);
        }

        public async Task<bool> CustomerIsOwner(Guid customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerType == CustomerType.Owner && c.Id == customerId);
        }

        public async Task<bool> CustomerIsRenter(Guid customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerType == CustomerType.Renter && c.Id == customerId);
        }

        public Customer? GetCustomerByNationalId(string nationalId)
        {
            return _context.Customers.FirstOrDefault(p => p.Person.NationalId == nationalId);
        }

        public async Task<int> GetCustomerContractsCount(Guid customerId)
        {
             var hasSaleContracts = await _context.Sales.CountAsync(s => s.BuyerId == customerId || s.SellerId == customerId);
             var hasRentalContracts = await _context.Rentals.CountAsync(s => s.LesseeId == customerId || s.LessorId == customerId);

            return hasSaleContracts + hasRentalContracts;
        }
            
        public async Task<bool> hasProperties(Guid ownerId)
        {
            return await _context.Properties.AnyAsync(p => p.OwnerId == ownerId);
        }

        public bool IsCustomerExists(string nationalId, CustomerType customerType)
        {
            return _context.Customers.Any(p => p.Person.NationalId == nationalId && p.CustomerType == customerType && !p.IsDeleted);
        }

        public bool IsCustomerExists(string nationalId)
        {
            return _context.Customers.Any(p => p.Person.NationalId == nationalId &&  !p.IsDeleted);
        }
        public bool IsCustomerExists(Guid ownerId)
        {
            return _context.Customers.Any(p => p.Id == ownerId &&  !p.IsDeleted);
        }

        public bool IsCustomerPhoneNumberAlreadyTaken(string phoneNumber)
        {
            return _context.Customers.GroupBy(p => p.PersonId).Any(g => g.Any( p => p.PhoneNumber == phoneNumber));
        }

         

    }
}
