using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.CustomerDTO
{
    public class FiltterCustomersDTO
    {
        public string? fullName { get; set; }
        public string? nationalId { get; set; }
        public string? gender { get; set; }
        public string? customerType { get; set; }
        public string? phoneNumber { get; set; }
    }
}
