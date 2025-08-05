using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RealEstate.API;
using RealEstate.API.Transformers;
using RealEstate.Application;
using RealEstate.Application.Common;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Common.Interfaces.Services;
using RealEstate.Infrastructure.Data;
using RealEstate.Infrastructure.Data.SeedData;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Repositorios.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWeb(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
 

builder.Services.AddSingleton<CustomAspNetCoreResultEndpointProfile>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.Configure<FileSettings>(
   builder.Configuration.GetSection("Files")
);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    //   Define the security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer abcdefgh12345\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    //   Apply the security globally (to all endpoints)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; 

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }); 
});

var app = builder.Build();
var profile = app.Services.GetRequiredService<CustomAspNetCoreResultEndpointProfile>();

AspNetCoreResult.Setup(config =>
{
    config.DefaultProfile = profile;
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseMigrationsEndPoint();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var fileManager = services.GetRequiredService<IFileManager>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var seeder = new SeedingData(services);
    context.Database.EnsureDeleted();
    context.Database.Migrate();

    await seeder.AddSeddingData(); 

}

app.Run();
 