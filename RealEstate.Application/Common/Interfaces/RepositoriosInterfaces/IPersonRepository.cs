using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces.RepositoriosInterfaces
{
    public interface IPersonRepository : IRepository<Person>
    {

        bool IsPersonExistsByNationalId(string nationalId);
        Person? GetPersonByNationalId(string nationalId);
    }
}
