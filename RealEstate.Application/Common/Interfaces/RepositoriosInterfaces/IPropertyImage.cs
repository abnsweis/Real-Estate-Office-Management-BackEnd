using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IPropertyImageRepository : IRepository<PropertyImage>
    {

        List<PropertyImage> GetImageByPropertyIdAsync(Guid propertyId);
    }
}
