using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common
{
    public class FileSettings
    {
        public string BaseUrl { get; set; } = "";
        public string Root { get; set; } = "Uploads";
        public Dictionary<string, string> FolderPaths { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AllowedExtensions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DefaultFiles { get; set; } = new Dictionary<string, string>();

    }
}
