using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Common.Interfaces
{
    public interface IAppEnvironmentService
    {
        string WebRootPath { get; }
        string ContentRootPath { get; }
        string EnvironmentName { get; }
    }
}
