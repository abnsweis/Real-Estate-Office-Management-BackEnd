using RealEstate.Domain.Entities;
namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IRentalsRepository : IRepository<Rental>
    { 
        bool IsRentalExistsById(Guid RentalId);
    }
}

