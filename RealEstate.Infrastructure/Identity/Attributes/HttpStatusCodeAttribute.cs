using RealEstate.Infrastructure.Identity.IdentiyErrors.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Identity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class HttpStatusCodeAttribute : Attribute
    {
        public enHttpStatusCode StatusCode { get; }
        public HttpStatusCodeAttribute(enHttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        
    }
}
