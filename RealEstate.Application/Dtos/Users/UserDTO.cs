using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Users
{
    public class UserDTO
    {
        public string? UserID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? NationalID { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? ImageUrl { get; set; }
        public string? phoneNumber { get; set; }
    }
}
