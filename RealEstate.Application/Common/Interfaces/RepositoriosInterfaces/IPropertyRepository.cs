using RealEstate.Domain.Entities;
using System.Linq.Expressions;
namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IPropertyRepository : IRepository<Property>
    {

        List<Property> GetOwnerProperties(Guid ownerId);
        List<Property> GetFeatured(Guid ownerId);
        Task<List<Property>> GetFeaturedPropertiesTop7(params Expression<Func<Property, object>>[]? includes);
        Task<List<Property>> GetLatestTop7Properties(params Expression<Func<Property, object>>[]? includes);
        Task<bool> IsPropertyyExistsByPropertyNumber(string propertyNumber);
        bool IsPropertyExistsById(Guid propertyId);
        Task<int> GetAvailablePropertiesCountAsync();
        Task<int> GetSoldPropertiesCountAsync(CancellationToken cancellationToken = default);

        Guid GetPropertyOwnerId(Guid propertyId);
        bool IsPropertyAvailable(Guid propertyId);
         
    }
}   
