using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common
{
    public static class Utils
    {


        public static string NormalizePhone(string? phone)
        {
            return string.IsNullOrEmpty(phone) ? string.Empty : new string(phone.Where(char.IsDigit).ToArray());
        }


        public static bool isGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid) || !Guid.TryParse(guid, out var _)) return false;
             
            return true;
        }

    }
}
