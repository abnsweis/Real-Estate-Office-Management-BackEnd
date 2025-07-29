using RealEstate.Application.Dtos.Ratings;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IRatingRepository :IRepository<Rating>
    {
        bool IsRatingExistsById(Guid RatingId);
        bool HasRatingByUser(Guid userId,Guid propertyId);
        Task<IEnumerable<RatingDTO>> GetAllWithUserNamesAsync(Guid propertyId, int PageNumber, int PageSize);
    }
}
