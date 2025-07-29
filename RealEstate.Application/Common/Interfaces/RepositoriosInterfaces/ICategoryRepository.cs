using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        bool IsCategoryExists(Guid categoryId);
        bool IsCategoryExists(string categoryName);
         
    }
}
