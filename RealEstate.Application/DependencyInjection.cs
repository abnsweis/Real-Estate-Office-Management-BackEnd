using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentResults.Extensions.AspNetCore;
using FluentValidation;
using System.Reflection;
using MediatR;
using RealEstate.Application.Common.Mappings.RealEstate.Application.Mappings;
using RealEstate.Application.Common.Behaviours;
using RealEstate.Application.Common.Services;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Security;
namespace RealEstate.Application
{


    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); 
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); 
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IFileManager, FileManager>();
            services.AddAutoMapper(typeof(UserDtoMappingProfile).Assembly);
            services.AddAutoMapper(typeof(AssemblyReference).Assembly);
            
            return services;
        }
    }
}
