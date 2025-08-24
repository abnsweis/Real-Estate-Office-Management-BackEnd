using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Entities
{
    public class Person : BaseAuditableEntity
    {
          

        public string FullName { get; set; } = null!;

        public string? NationalId { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        public string ImageURL { get; set; } = null!;

        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
