using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Data;
using System.Linq.Expressions;

namespace RealEstate.Infrastructure.Repositorios
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

        public List<Property> GetFeatured(Guid ownerId)
        {
            throw new NotImplementedException();
        }

        public List<Property> GetOwnerProperties(Guid ownerId)
        {
            return _context.Properties.Where(p => p.OwnerId == ownerId && !p.IsDeleted).ToList();
        }

        public List<Property> GetOwnerPropertiesByNationalId(string nationalId)
        {
            return _context.Properties.Where(p => p.Owner.Person.NationalId == nationalId && !p.IsDeleted).ToList();
        }

 

        public Task<List<Property>> GetFeaturedPropertiesTop7(params Expression<Func<Property, object>>[]? includes)
        {
            IQueryable<Property> query = _context.Properties.AsQueryable();

            // Include specified navigation properties
            foreach (Expression<Func<Property, object>> include in includes)
            {
                query = query.Include(include);
            }
 
            return query.OrderByDescending(p => p.Ratings.Any() ? p.Ratings.Average(r => r.RatingNumber) : 0).Take(7).ToListAsync();
        }
        public Task<List<Property>> GetLatestTop7Properties(params Expression<Func<Property, object>>[]? includes)
        {
            IQueryable<Property> query = _context.Properties.AsQueryable();

            // Include specified navigation properties
            foreach (Expression<Func<Property, object>> include in includes)
            {
                query = query.Include(include);
            }

            return query.OrderByDescending(p => p.CreatedDate).Take(7).ToListAsync();
        }
        public Guid GetPropertyOwnerId(Guid propertyId)
        {
            return _context.Properties.FirstOrDefault(p => p.Id == propertyId)!.OwnerId   ;
        }

        public bool IsPropertyAvailable(Guid propertyId)
        {
            return _context.Properties.FirstOrDefault(p => p.Id == propertyId)!.PropertyStatus == enPropertyStatus.Available;
        }

        public bool IsPropertyExistsById(Guid propertyId)
        {
            return _context.Properties.Any(p => p.Id == propertyId && !p.IsDeleted);
        }

        public async Task<bool> IsPropertyyExistsByPropertyNumber(string propertyNumber)
        {
            return await _context.Properties.AnyAsync(p => p.PropertyNumber == propertyNumber && !p.IsDeleted);
        }

        
    }
}
