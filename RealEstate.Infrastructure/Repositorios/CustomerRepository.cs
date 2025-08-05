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

        public Customer? GetCustomerByNationalId(string nationalId)
        {
            return _context.Customers.FirstOrDefault(p => p.Person.NationalId == nationalId);
        }

        public bool IsCustomerExists(string nationalId, enCustomerType customerType)
        {
            return _context.Customers.Any(p => p.Person.NationalId == nationalId && p.CustomerType == customerType);
        }

        public bool IsCustomerExists(string nationalId)
        {
            return _context.Customers.Any(p => p.Person.NationalId == nationalId);
        }
        public bool IsCustomerExists(Guid ownerId)
        {
            return _context.Customers.Any(p => p.Id == ownerId);
        }

        public bool IsCustomerPhoneNumberAlreadyTaken(string phoneNumber)
        {
            return _context.Customers.GroupBy(p => p.PersonId).Any(g => g.Any( p => p.PhoneNumber == phoneNumber));
        }

         

    }
}
