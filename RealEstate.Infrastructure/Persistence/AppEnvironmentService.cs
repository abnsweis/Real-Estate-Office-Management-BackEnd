using Microsoft.AspNetCore.Hosting;
using RealEstate.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Persistence
{
    internal class AppEnvironmentService : IAppEnvironmentService
    {
        private readonly IWebHostEnvironment _evn;

        public AppEnvironmentService(IWebHostEnvironment evn)
        {
            this._evn = evn;
        }
        public string WebRootPath => _evn.WebRootPath;

        public string ContentRootPath => _evn.ContentRootPath;

        public string EnvironmentName => _evn.EnvironmentName;
    }
}
