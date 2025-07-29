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
    public class CommantsRepository : Repository<Comment>, ICommantsRepository
    {
        private readonly ApplicationDbContext _context;

        public CommantsRepository(ApplicationDbContext context) : base(context)  
        {
            this._context = context;
        }
 
    }
}
