using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        public Guid? UserId { get; }
        public string? Username { get; }
    }
}
