using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Users
{
    public class FiltterUserDTO
        {
        public string? FullName { get; set; } 
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Phone { get; set; } 
    }
}
