using Microsoft.EntityFrameworkCore;
using Persistence;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args); // to create something called a Kestrel server.

// Add services to the container.

// ad policy that can be applied as a whole
// And this effectively means that now every single controller endpoint is going to require authentication.
builder.Services.AddControllers(opt => 
{
     var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
     opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration); // related to authService

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");
app.UseAuthentication();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");


// using statement means that when we're finished with this scope, anything inside it is going to be disposed or destroyed and cleaned up from memory. like garbage collector
// So we need to get access to a services and we create a scope specifically to do this because our service
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try //create database
{
  var context = services.GetRequiredService<DataContext>();
  var userManager = services.GetRequiredService<UserManager<AppUser>>();
  await context.Database.MigrateAsync();// it will create db if it does not exist
  await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>(); //  we told the logger what class we're going to be logging against.
  logger.LogError(ex, "An error occured during migration");
}

app.Run();


// DataContext is what class we use for our db context

// add sqlite connection string  in app settings.development.json file And SQLite just uses a file to store a database and typically it uses a DB extension.