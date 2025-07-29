using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Entities
{
    public class UserDomain
    {

        public UserDomain()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string PhoneNumber { get; set; } 
        public Person Person { get; set; }

        private bool _isDeleted;


        public void Delete() {
        
            _isDeleted = true;
        }
    }
}
