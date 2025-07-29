using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser<Guid>,ICurrentUserService 
    {
        public Guid personId { get; set; }
        public Person Person { get; set; }
        public bool IsDeleted { get; set; }

        public Guid? UserId => base.Id;

        public string? Username => base.UserName;
    }
}
