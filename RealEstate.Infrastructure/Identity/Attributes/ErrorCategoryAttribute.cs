using RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorCategoryAttribute : Attribute
    {
        public enErrorCategory Category { get; }
        public ErrorCategoryAttribute(enErrorCategory category)
        {
            Category = category;
        }
    }
}
