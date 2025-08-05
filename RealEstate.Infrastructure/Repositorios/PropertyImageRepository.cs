using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Repositorios
{
    public class PropertyImageRepository : Repository<PropertyImage>, IPropertyImageRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyImageRepository(ApplicationDbContext context) : base(context) 
        {
            this._context = context;
        }

        public void Delete(Guid propertyId)
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM PropertyImages WHERE PropertyID = {0}", propertyId);
        }

        public List<PropertyImage> GetImageByPropertyIdAsync(Guid propertyId)
        {
            return _context.PropertyImages.Where(pi => pi.PropertyId == propertyId).ToList();
        }
    }
}
