using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositorios
{
    public class SalesRepository : Repository<Sale>, ISalesRepository
    {
        private readonly ApplicationDbContext _context;

        public SalesRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

        public bool IsSaleExistsById(Guid SaleId)
        {
            return _context.Sales.Any(p => p.Id == SaleId && !p.IsDeleted);
        }
    }
}
