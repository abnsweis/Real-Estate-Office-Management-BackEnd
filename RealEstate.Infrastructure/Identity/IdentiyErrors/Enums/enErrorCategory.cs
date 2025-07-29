using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;

public enum enErrorCategory
{
    Password,
    Username,
    Email,
    Role,
    User,
    Token,
    Concurrency,
    Authentication,
    Default
}
 