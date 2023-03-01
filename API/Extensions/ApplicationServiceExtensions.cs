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
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen();

      services.AddDbContext<DataContext>(opt =>
      {
        opt.UseSqlite(
            config.GetConnectionString("DefaultConnection"));
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