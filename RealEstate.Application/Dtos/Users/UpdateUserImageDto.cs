using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Dtos.Users
{
    public class UpdateUserImageDto
    {
        public IFormFile? Image { get; set; }
    }
}
