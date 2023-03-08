using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Activities;
using Application.Core;
using FluentValidation;
using FluentValidation.AspNetCore;
using Application.Interfaces;
using Infrastructure.Security;
using Infrastructure.Photos;


namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {

      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen();
             /*  services.AddDbContext<DataContext>(opt =>
                 {
                   opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));     

                 });  */

      services.AddDbContext<DataContext>(options =>
              {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                if (env == "Development")
                {
                  connStr = config.GetConnectionString("DefaultConnection"); // DefaultConnection string must match appsettings.Development.json
                }
                else
                {
                  // Use connection string provided at runtime by FlyIO.
                  var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                  // Parse connection URL to connection string for Npgsql
                  connUrl = connUrl.Replace("postgres://", string.Empty);

                  var pgUserPass = connUrl.Split("@")[0];
                  var pgHostPortDb = connUrl.Split("@")[1];
                  var pgHostPort = pgHostPortDb.Split("/")[0];
                  var pgDb = pgHostPortDb.Split("/")[1];
                  var pgUser = pgUserPass.Split(":")[0];
                  var pgPass = pgUserPass.Split(":")[1];
                  var pgHost = pgHostPort.Split(":")[0];
                  var pgPort = pgHostPort.Split(":")[1];

                  connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
                }

                options.UseNpgsql(connStr);
              });





      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
         {
           policy
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithOrigins("http://localhost:3000")
                      .AllowCredentials();
         });
      });
      services.AddMediatR(typeof(List.Handler));
      services.AddAutoMapper(typeof(MappingProfiles).Assembly); // register MappingPrifiles as a service


      services.AddFluentValidationAutoValidation();
      services.AddValidatorsFromAssemblyContaining<Create>();
      services.AddHttpContextAccessor(); // we need to add a service for the HTTP context accesses. So that we can utilize that inside our infrastructure project.
      services.AddScoped<IUserAccessor, UserAccessor>(); // this will make this available to be injected inside our application handlers.
      services.AddScoped<IPhotoAccessor, PhotoAccessor>();
      services.Configure<CloudinarySettings>(config.GetSection("Cloudinary")); // the name Cloudinary must match what we call it in appsettings.json
      services.AddSignalR();

      return services;

    }
  }
}