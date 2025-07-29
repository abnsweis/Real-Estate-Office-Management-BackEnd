using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstate.API.Services;
using RealEstate.Application.Common;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Application.Common.Services;
using RealEstate.Application.Security;
using System.Reflection;
using System.Text;

namespace RealEstate.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration config)
        {
            // Inject HttpContext
            services.AddHttpContextAccessor();

            // Inject CurrentUserService
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });




            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
             
 


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,


                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]))
                };
            });
            ;

;
            services.AddAuthorization();

 

            return services;
        }
         
    }
}
