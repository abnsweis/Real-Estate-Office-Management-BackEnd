using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Repositorios
{
    public class FavoritesRepository : Repository<Favorite>, IFavoritesRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoritesRepository(ApplicationDbContext context) :base(context) 
        {
            this._context = context;
        }

        public bool IsInFavorite(Guid propertyId,Guid userId)
        {
            return _context.Favorites.Any(f => f.PropertyId == propertyId && f.UserId == userId);
        }
    }
}
