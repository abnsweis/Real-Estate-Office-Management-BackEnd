using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer? GetCustomerByNationalId(string nationalId);
        bool IsCustomerPhoneNumberAlreadyTaken(string phoneNumber);
        bool IsCustomerExists(string nationalId, enCustomerType customerType);
        bool IsCustomerExists(string nationalId);
        bool IsCustomerExists(Guid ownerId);
        Task<int> CountCreatedInCurrentMonth();

    }
}
