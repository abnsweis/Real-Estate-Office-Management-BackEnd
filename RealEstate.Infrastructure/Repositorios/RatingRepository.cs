using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Application.Dtos.Ratings;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositorios
{
    public class RatingRepository : Repository<Rating> , IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }


        public async Task<IEnumerable<RatingDTO>> GetAllWithUserNamesAsync(Guid propertyId,int PageNumber, int PageSize)
        {

            var ratings = from r in _context.Ratings
                          join u in _context.Users on r.UserId equals u.Id
                          join p in _context.People on u.personId equals p.Id
                          where r.Id == propertyId
                          select new RatingDTO
                          {
                              RatinId = r.Id.ToString(),
                              UserId = u.Id.ToString(),
                              PropertyId = p.Id.ToString(),
                              RatingText = r.RatingText,
                              RatingNumber = r.RatingNumber.ToString(),
                              FullName = p.FullName,
                              Username = u.UserName,
                              ImageURL = p.ImageURL

                          };


            int skip = (PageNumber - 1) * PageSize;
            ratings = ratings.Skip(skip).Take(PageSize);
            return await ratings.ToListAsync();

        }

        public bool HasRatingByUser(Guid userId, Guid propertyId)
        {
            return _context.Ratings.Any(r => r.UserId == userId && r.PropertyId == propertyId);
        }

        public bool IsRatingExistsById(Guid RatingId)
        {
            return _context.Ratings.Any(p => p.Id == RatingId && !p.IsDeleted);
        }
         
    }
}
