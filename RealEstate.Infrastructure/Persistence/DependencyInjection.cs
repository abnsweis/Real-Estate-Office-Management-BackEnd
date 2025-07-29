using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.RepositoriosInterfaces;
using RealEstate.Infrastructure.Data;
using RealEstate.Infrastructure.Data.Interceptors;
using RealEstate.Infrastructure.Data.SeedData;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.Identity.jwt;
using RealEstate.Infrastructure.Persistence.Mappings;
using RealEstate.Infrastructure.Repositorios;
using RealEstate.Infrastructure.Repositorios.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();  
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>();
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")).AddInterceptors(interceptor);
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddSingleton<TimeProvider>(_ => TimeProvider.System);

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper(cfg =>
            { 
                cfg.AddProfile(new UserMappingProfile());
                cfg.AddExpressionMapping();
            });

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAppEnvironmentService, AppEnvironmentService>();
            services.AddScoped<SeedingData>();
            services.AddScoped<JwtTokenGenerator>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>(); 
            services.AddScoped<IPropertyImageRepository, PropertyImageRepository>(); 
            services.AddScoped<ISalesRepository, SalesRepository>(); 
            services.AddScoped<IRentalsRepository, RentalsRepository>(); 
            services.AddScoped<ITestimonialsRepository, TestimonialsRepository>(); 
            services.AddScoped<IRatingRepository, RatingRepository>(); 
            services.AddScoped<ICommantsRepository, CommantsRepository>(); 
            services.AddScoped<IFavoritesRepository, FavoritesRepository>(); 


            return services;
        }
         
    }
}
