using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositorios
{
    public class RentalsRepository : Repository<Rental>, IRentalsRepository
    {
        private readonly ApplicationDbContext _context;
        
        public RentalsRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

        public bool IsRentalExistsById(Guid RentalId)
        {
            return _context.Rentals.Any(p => p.Id == RentalId && !p.IsDeleted);
        }
    }
}
