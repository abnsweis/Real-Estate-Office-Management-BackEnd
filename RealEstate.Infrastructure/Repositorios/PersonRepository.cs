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
    public class PersonRepository : Repository<Person> , IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context) : base(context)
        {
            this._context = context;
        }

        public Person? GetPersonByNationalId(string nationalId)
        {
            return _context.People.FirstOrDefault(p => p.NationalId == nationalId);
        }

        public bool IsPersonExistsByNationalId(string nationalId)
        {
            return _context.People.Any(p => p.NationalId == nationalId);
        }
    }
}
