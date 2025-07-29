using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IFavoritesRepository : IRepository<Favorite>
    {
        bool IsInFavorite(Guid propertyId, Guid userId);
    }
}
