using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer? GetCustomerByNationalId(string nationalId);
        bool IsCustomerPhoneNumberAlreadyTaken(string phoneNumber);
        bool IsCustomerExists(string nationalId, CustomerType customerType);
        bool IsCustomerExists(string nationalId);
        bool IsCustomerExists(Guid ownerId);
        Task<bool> CustomerIsOwner(Guid customerId);
        Task<bool> CustomerIsRenter(Guid customerId);
        Task<bool> CustomerIsBuyer(Guid customerId);
        Task<bool> hasProperties(Guid ownerId);
        Task<int> CountCreatedInCurrentMonth();
        Task<int> GetCustomerContractsCount(Guid customerId);

    }
}
