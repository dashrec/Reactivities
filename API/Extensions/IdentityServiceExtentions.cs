using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
  public static class IdentityServiceExtentions
  {
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
      services.AddIdentityCore<AppUser>(opt =>
      {
        opt.Password.RequireNonAlphanumeric = false;
        opt.User.RequireUniqueEmail = true; // check for duplicate email. 

      }).AddEntityFrameworkStores<DataContext>();


      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])); // the key that we pass in here needs to match exactly what we have in our token service.

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(opt =>
          {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = key,
              ValidateIssuer = false,
              ValidateAudience = false
            };

            opt.Events = new JwtBearerEvents
            {
              OnMessageReceived = context =>
                    {
                      var accessToken = context.Request.Query["access_token"];  // the client side is going to pass our access_token in a query string.
                      var path = context.HttpContext.Request.Path;
                      if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat"))) // chat is an endpoint of our signalHub
                      {
                        context.Token = accessToken; // inside our hub context we will have excess to this Token
                      }
                      return Task.CompletedTask;
                    }
            };
          });




      // with this policy only the host of the activcity can edit the activity
      services.AddAuthorization(opt =>
      {
        opt.AddPolicy("IsActivityHost", policy =>
        {
          policy.Requirements.Add(new IsHostRequirement());

        });
      });
      services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
      services.AddScoped<TokenService>();
      //  with this in place we can use IsActivityHost om endpoints

      return services;
    }
  }
}

// So opt.password has some password options

//AddEntityFrameworkStores <-- What this does is effectively allows us to query our users in the Entity Framework Store or our database, for example.