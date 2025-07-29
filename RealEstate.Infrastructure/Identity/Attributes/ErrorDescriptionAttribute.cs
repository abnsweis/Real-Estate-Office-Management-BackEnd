using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class ErrorDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public ErrorDescriptionAttribute(string description)
        {
            Description = description;
        }

    }
}
